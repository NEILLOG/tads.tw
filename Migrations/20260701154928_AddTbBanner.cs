using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbBanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbBanner",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FileID = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LinkUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TbBanner", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "TbIdSummary",
                columns: new[] { "TableName", "Prefix", "Length", "MaxID" },
                values: new object[] { "TbBanner", "BN", 10, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TbIdSummary",
                keyColumn: "TableName",
                keyValue: "TbBanner");

            migrationBuilder.DropTable(
                name: "TbBanner");
        }
    }
}
