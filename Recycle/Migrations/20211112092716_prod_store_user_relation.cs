using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class prod_store_user_relation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UserId",
                table: "Stores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId",
                table: "Products",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Stores_StoreId",
                table: "Products",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Stores_StoreId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Users_UserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_UserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Products");
        }
    }
}
