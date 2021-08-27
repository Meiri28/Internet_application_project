using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class adminuserconnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admin");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Admin",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admin_UserId",
                table: "Admin",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_User_UserId",
                table: "Admin",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_User_UserId",
                table: "Admin");

            migrationBuilder.DropIndex(
                name: "IX_Admin_UserId",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Admin");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
