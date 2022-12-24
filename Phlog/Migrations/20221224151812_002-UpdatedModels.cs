using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phlog.Migrations
{
    /// <inheritdoc />
    public partial class _002UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Tag_TagId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_TagId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Post");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tag_PostId",
                table: "Tag",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Post_PostId",
                table: "Tag",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Post_PostId",
                table: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Tag_PostId",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Tag");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Post",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Post_TagId",
                table: "Post",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Tag_TagId",
                table: "Post",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
