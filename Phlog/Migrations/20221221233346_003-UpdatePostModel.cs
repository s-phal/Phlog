using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phlog.Migrations
{
    /// <inheritdoc />
    public partial class _003UpdatePostModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Post",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Post");
        }
    }
}
