using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue");

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue");

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_Track_PlayingTrackId",
                table: "Queue",
                column: "PlayingTrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
