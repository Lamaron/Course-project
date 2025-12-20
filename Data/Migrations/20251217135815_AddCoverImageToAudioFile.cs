using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverImageToAudioFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioFiles_Albums_Album_Id",
                table: "AudioFiles");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_AudioFiles_Album_Id",
                table: "AudioFiles");

            migrationBuilder.DropColumn(
                name: "Album_Id",
                table: "AudioFiles");

            migrationBuilder.AddColumn<byte[]>(
                name: "CoverImage",
                table: "AudioFiles",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "AudioFiles");

            migrationBuilder.AddColumn<int>(
                name: "Album_Id",
                table: "AudioFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_Album_Id",
                table: "AudioFiles",
                column: "Album_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioFiles_Albums_Album_Id",
                table: "AudioFiles",
                column: "Album_Id",
                principalTable: "Albums",
                principalColumn: "Id");
        }
    }
}
