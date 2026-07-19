using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Groupadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastMessageAt",
                table: "Conversations",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "LastMessage",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ConversationType",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Conversations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdName",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantRole",
                table: "ConversationParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId1",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReplyToId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_IdName",
                table: "Conversations",
                column: "IdName",
                unique: true,
                filter: "[IdName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId1",
                table: "ChatMessages",
                column: "ConversationId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReplyToId",
                table: "ChatMessages",
                column: "ReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatMessages_ReplyToId",
                table: "ChatMessages",
                column: "ReplyToId",
                principalTable: "ChatMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId1",
                table: "ChatMessages",
                column: "ConversationId1",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatMessages_ReplyToId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_IdName",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ReplyToId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ConversationType",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IdName",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ParticipantRole",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "ConversationId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ReplyToId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastMessageAt",
                table: "Conversations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastMessage",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Conversations_ConversationId",
                table: "ChatMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
