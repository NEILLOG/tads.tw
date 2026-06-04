using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbApiLog",
                columns: table => new
                {
                    PID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Request = table.Column<string>(type: "TEXT", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: false),
                    RoutePath = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddr = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_API_Log", x => x.PID);
                });

            migrationBuilder.CreateTable(
                name: "TbBackendOperateLog",
                columns: table => new
                {
                    PID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Feature = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsSuccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataKey = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Request = table.Column<string>(type: "TEXT", nullable: true),
                    Response = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOperateLog", x => x.PID);
                });

            migrationBuilder.CreateTable(
                name: "TbFileInfo",
                columns: table => new
                {
                    FileID = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    FileRealName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    FileDescription = table.Column<string>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FilePathM = table.Column<string>(type: "TEXT", nullable: true),
                    FilePathS = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<byte>(type: "INTEGER", nullable: true),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_FileInfo", x => x.FileID);
                });

            migrationBuilder.CreateTable(
                name: "TbIdSummary",
                columns: table => new
                {
                    TableName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", nullable: false),
                    Length = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxID = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbIdSummary_1", x => x.TableName);
                });

            migrationBuilder.CreateTable(
                name: "TbLog",
                columns: table => new
                {
                    PID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LoginRecord", x => x.PID);
                });

            migrationBuilder.CreateTable(
                name: "TbLoginRecord",
                columns: table => new
                {
                    PID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Account = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    MemberID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Inputaua8 = table.Column<string>(type: "TEXT", nullable: true),
                    IP = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    LoginTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LoginMsg = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SSO = table.Column<bool>(type: "INTEGER", nullable: false),
                    SSOResult = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_S_LoginRecord", x => x.PID);
                });

            migrationBuilder.CreateTable(
                name: "TbNews",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DisplayDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Contents = table.Column<string>(type: "TEXT", nullable: false),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublish = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsKeepTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileID = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbNews", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbApiLog");

            migrationBuilder.DropTable(
                name: "TbBackendOperateLog");

            migrationBuilder.DropTable(
                name: "TbFileInfo");

            migrationBuilder.DropTable(
                name: "TbIdSummary");

            migrationBuilder.DropTable(
                name: "TbLog");

            migrationBuilder.DropTable(
                name: "TbLoginRecord");

            migrationBuilder.DropTable(
                name: "TbNews");
        }
    }
}
