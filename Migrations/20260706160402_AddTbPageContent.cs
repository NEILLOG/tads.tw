using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbPageContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbPageContent",
                columns: table => new
                {
                    PageCode = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    PageName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Contents = table.Column<string>(type: "TEXT", nullable: true),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbPageContent", x => x.PageCode);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbPageContent");
        }
    }
}
