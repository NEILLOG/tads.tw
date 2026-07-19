using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbAboutContentMembershipFormFileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MembershipFormFileId",
                table: "TbAboutContent",
                type: "TEXT",
                maxLength: 15,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MembershipFormFileId",
                table: "TbAboutContent");
        }
    }
}
