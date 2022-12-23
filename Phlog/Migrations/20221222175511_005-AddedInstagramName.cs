using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phlog.Migrations
{
    /// <inheritdoc />
    public partial class _005AddedInstagramName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Post",
                newName: "InstagramUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InstagramUsername",
                table: "Post",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Post",
                type: "text",
                nullable: true);
        }
    }
}
