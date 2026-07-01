using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedTbFileInfoIdSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TbIdSummary",
                columns: new[] { "TableName", "Prefix", "Length", "MaxID" },
                values: new object[] { "TbFileInfo", "FI", 12, 1L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TbIdSummary",
                keyColumn: "TableName",
                keyValue: "TbFileInfo");
        }
    }
}
