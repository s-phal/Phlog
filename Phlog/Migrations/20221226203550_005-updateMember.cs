using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phlog.Migrations
{
    /// <inheritdoc />
    public partial class _005updateMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserBio",
                table: "AspNetUsers",
                newName: "AboutMe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AboutMe",
                table: "AspNetUsers",
                newName: "UserBio");
        }
    }
}
