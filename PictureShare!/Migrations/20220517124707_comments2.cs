using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PictureShare_.Migrations
{
    public partial class comments2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PictureModelId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PictureModelId",
                table: "Comments",
                column: "PictureModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Pictures_PictureModelId",
                table: "Comments",
                column: "PictureModelId",
                principalTable: "Pictures",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Pictures_PictureModelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PictureModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PictureModelId",
                table: "Comments");
        }
    }
}
