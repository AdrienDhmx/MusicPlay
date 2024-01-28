using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlay.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumTag_Album_Id2",
                table: "AlbumTag");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumTag_Tag_Id1",
                table: "AlbumTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistGroupMember_Artist_Id1",
                table: "ArtistGroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistGroupMember_Artist_Id2",
                table: "ArtistGroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistTag_Artist_Id2",
                table: "ArtistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistTag_Tag_Id1",
                table: "ArtistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTag_Playlist_Id2",
                table: "PlaylistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTag_Tag_Id1",
                table: "PlaylistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlist_Id1",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Track_Id2",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Queue_Id1",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Track_Id2",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTag_Tag_Id1",
                table: "TrackTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTag_Track_Id2",
                table: "TrackTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack");

            migrationBuilder.DropIndex(
                name: "IX_QueueTrack_Id2",
                table: "QueueTrack");

            migrationBuilder.DropColumn(
                name: "Id1",
                table: "QueueTrack");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "TrackTag",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "TrackTag",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTag_Id1",
                table: "TrackTag",
                newName: "IX_TrackTag_TrackId");

            migrationBuilder.RenameColumn(
                name: "IsPlaying",
                table: "QueueTrack",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "QueueTrack",
                newName: "QueueId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "PlaylistTrack",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "PlaylistTrack",
                newName: "PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_Id2",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_TrackId");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "PlaylistTag",
                newName: "PlaylistId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "PlaylistTag",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTag_Id1",
                table: "PlaylistTag",
                newName: "IX_PlaylistTag_PlaylistId");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "ArtistTag",
                newName: "ArtistId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "ArtistTag",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistTag_Id1",
                table: "ArtistTag",
                newName: "IX_ArtistTag_ArtistId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "ArtistGroupMember",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "ArtistGroupMember",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistGroupMember_Id2",
                table: "ArtistGroupMember",
                newName: "IX_ArtistGroupMember_MemberId");

            migrationBuilder.RenameColumn(
                name: "Id1",
                table: "AlbumTag",
                newName: "AlbumId");

            migrationBuilder.RenameColumn(
                name: "Id2",
                table: "AlbumTag",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumTag_Id1",
                table: "AlbumTag",
                newName: "IX_AlbumTag_AlbumId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack",
                columns: new[] { "QueueId", "TrackId" });

            migrationBuilder.CreateIndex(
                name: "IX_QueueTrack_TrackId",
                table: "QueueTrack",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumTag_Album_AlbumId",
                table: "AlbumTag",
                column: "AlbumId",
                principalTable: "Album",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumTag_Tag_TagId",
                table: "AlbumTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistGroupMember_Artist_GroupId",
                table: "ArtistGroupMember",
                column: "GroupId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistGroupMember_Artist_MemberId",
                table: "ArtistGroupMember",
                column: "MemberId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTag_Artist_ArtistId",
                table: "ArtistTag",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTag_Tag_TagId",
                table: "ArtistTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTag_Playlist_PlaylistId",
                table: "PlaylistTag",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTag_Tag_TagId",
                table: "PlaylistTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                principalTable: "QueueTrack",
                principalColumns: new[] { "QueueId", "TrackId" },
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Queue_QueueId",
                table: "QueueTrack",
                column: "QueueId",
                principalTable: "Queue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Track_TrackId",
                table: "QueueTrack",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTag_Tag_TagId",
                table: "TrackTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTag_Track_TrackId",
                table: "TrackTag",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumTag_Album_AlbumId",
                table: "AlbumTag");

            migrationBuilder.DropForeignKey(
                name: "FK_AlbumTag_Tag_TagId",
                table: "AlbumTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistGroupMember_Artist_GroupId",
                table: "ArtistGroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistGroupMember_Artist_MemberId",
                table: "ArtistGroupMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistTag_Artist_ArtistId",
                table: "ArtistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistTag_Tag_TagId",
                table: "ArtistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTag_Playlist_PlaylistId",
                table: "PlaylistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTag_Tag_TagId",
                table: "PlaylistTag");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlist_PlaylistId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Track_TrackId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Queue_QueueId",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Track_TrackId",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTag_Tag_TagId",
                table: "TrackTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackTag_Track_TrackId",
                table: "TrackTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack");

            migrationBuilder.DropIndex(
                name: "IX_QueueTrack_TrackId",
                table: "QueueTrack");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "TrackTag",
                newName: "Id1");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "TrackTag",
                newName: "Id2");

            migrationBuilder.RenameIndex(
                name: "IX_TrackTag_TrackId",
                table: "TrackTag",
                newName: "IX_TrackTag_Id1");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "QueueTrack",
                newName: "IsPlaying");

            migrationBuilder.RenameColumn(
                name: "QueueId",
                table: "QueueTrack",
                newName: "Id2");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "PlaylistTrack",
                newName: "Id2");

            migrationBuilder.RenameColumn(
                name: "PlaylistId",
                table: "PlaylistTrack",
                newName: "Id1");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_TrackId",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_Id2");

            migrationBuilder.RenameColumn(
                name: "PlaylistId",
                table: "PlaylistTag",
                newName: "Id1");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "PlaylistTag",
                newName: "Id2");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTag_PlaylistId",
                table: "PlaylistTag",
                newName: "IX_PlaylistTag_Id1");

            migrationBuilder.RenameColumn(
                name: "ArtistId",
                table: "ArtistTag",
                newName: "Id1");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "ArtistTag",
                newName: "Id2");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistTag_ArtistId",
                table: "ArtistTag",
                newName: "IX_ArtistTag_Id1");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "ArtistGroupMember",
                newName: "Id2");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "ArtistGroupMember",
                newName: "Id1");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistGroupMember_MemberId",
                table: "ArtistGroupMember",
                newName: "IX_ArtistGroupMember_Id2");

            migrationBuilder.RenameColumn(
                name: "AlbumId",
                table: "AlbumTag",
                newName: "Id1");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "AlbumTag",
                newName: "Id2");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumTag_AlbumId",
                table: "AlbumTag",
                newName: "IX_AlbumTag_Id1");

            migrationBuilder.AddColumn<int>(
                name: "Id1",
                table: "QueueTrack",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack",
                columns: new[] { "Id1", "Id2" });

            migrationBuilder.CreateIndex(
                name: "IX_QueueTrack_Id2",
                table: "QueueTrack",
                column: "Id2");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumTag_Album_Id2",
                table: "AlbumTag",
                column: "Id2",
                principalTable: "Album",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumTag_Tag_Id1",
                table: "AlbumTag",
                column: "Id1",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistGroupMember_Artist_Id1",
                table: "ArtistGroupMember",
                column: "Id1",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistGroupMember_Artist_Id2",
                table: "ArtistGroupMember",
                column: "Id2",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTag_Artist_Id2",
                table: "ArtistTag",
                column: "Id2",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistTag_Tag_Id1",
                table: "ArtistTag",
                column: "Id1",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTag_Playlist_Id2",
                table: "PlaylistTag",
                column: "Id2",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTag_Tag_Id1",
                table: "PlaylistTag",
                column: "Id1",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlist_Id1",
                table: "PlaylistTrack",
                column: "Id1",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Track_Id2",
                table: "PlaylistTrack",
                column: "Id2",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Queue_QueueTrack_Id_PlayingTrackId",
                table: "Queue",
                columns: new[] { "Id", "PlayingTrackId" },
                principalTable: "QueueTrack",
                principalColumns: new[] { "Id1", "Id2" },
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Queue_Id1",
                table: "QueueTrack",
                column: "Id1",
                principalTable: "Queue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Track_Id2",
                table: "QueueTrack",
                column: "Id2",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTag_Tag_Id1",
                table: "TrackTag",
                column: "Id1",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackTag_Track_Id2",
                table: "TrackTag",
                column: "Id2",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
