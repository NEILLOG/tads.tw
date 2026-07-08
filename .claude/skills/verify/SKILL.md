---
name: verify
description: Build, launch, and drive the TADS_Web ASP.NET Core site to verify changes end-to-end.
---

# TADS_Web 驗證流程

## 建置與啟動

```bash
dotnet build                                   # 既有 ~152 個 nullable 警告屬正常
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-build --urls http://localhost:31793
# 背景啟動後輪詢 http://localhost:31793/ 直到回 200（約數秒）
```

- 開發環境資料庫：SQLite `tads_dev.db`（正式為 `tads.db`），可直接用 `sqlite3` 查驗。
- 啟動時 Program.cs 會跑 Seeder（PageContentSeeder、AboutContentSeeder 等），冪等、只補缺漏列。

## 常用驗證路徑

- 前台頁面免登入：`curl -s http://localhost:31793/<path>` 直接抓 HTML grep 標記。
- 後台（/PageContent/List 等）需要 Session 登入，直接 curl 會被導向；驗證後台編輯效果可改用
  `sqlite3 tads_dev.db "UPDATE TbPageContent SET Contents=... WHERE PageCode=...;"` 模擬，
  改完 curl 前台確認，再還原（伺服器不快取，DB 改動即時反映）。
- 內頁機制：TbPageContent 一頁一筆（PageCode 主鍵），前台 View 以 `@Html.Raw` 輸出，
  空值時退回 View 內建靜態 fallback。

## 注意

- `tads_dev.db` / `tads.db` 常出現在 git status 的已修改清單，屬正常（seeder 與操作日誌會寫入）。
- 驗證用的暫時 DB 改動記得還原，避免污染開發資料；上傳測試檔要一併刪 TbFileInfo 列與 `wwwroot/upload/` 下實體檔。
- 沙箱內 `curl -F "f=@file"` 會失敗（error 26 讀不到檔案）；表單/檔案上傳測試改用 python3 標準庫手組 multipart（CookieJar 抓 session + 從頁面 regex 出 `__RequestVerificationToken`）。
- 後台 flash 訊息（SweetAlert）中文在 HTML 內是 JSON `\uXXXX` 逸出、清單頁中文是 `&#x...;` entity，grep 中文常誤判為沒出現，要用逸出後的字串比對。
