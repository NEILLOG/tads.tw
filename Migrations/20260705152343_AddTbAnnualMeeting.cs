using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbAnnualMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbAnnualMeeting",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Contents = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublish = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAnnualMeeting", x => x.ID);
                });

            // IDGenerator 需要 TbIdSummary 有對應資料才能產生流水號
            migrationBuilder.InsertData(
                table: "TbIdSummary",
                columns: new[] { "TableName", "Prefix", "Length", "MaxID" },
                values: new object[] { "TbAnnualMeeting", "AM", 10, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbAnnualMeeting");

            migrationBuilder.DeleteData(
                table: "TbIdSummary",
                keyColumn: "TableName",
                keyValue: "TbAnnualMeeting");
        }
    }
}
