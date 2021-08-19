using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class userGender2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserGender_GenderId",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "GenderId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserGender_GenderId",
                table: "User",
                column: "GenderId",
                principalTable: "UserGender",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserGender_GenderId",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "GenderId",
                table: "User",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserGender_GenderId",
                table: "User",
                column: "GenderId",
                principalTable: "UserGender",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
