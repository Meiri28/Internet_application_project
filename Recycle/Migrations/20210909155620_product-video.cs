using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class productvideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoURL",
                table: "Product");

            migrationBuilder.AddColumn<byte[]>(
                name: "Video",
                table: "Product",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Video",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "VideoURL",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
