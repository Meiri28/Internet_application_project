using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class productImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsComment_Product_ProductsId",
                table: "ProductsComment");

            migrationBuilder.DropColumn(
                name: "Pictures",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ProductsId",
                table: "ProductsComment",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductsComment_ProductsId",
                table: "ProductsComment",
                newName: "IX_ProductsComment_ProductId");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "Product",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsComment_Product_ProductId",
                table: "ProductsComment",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsComment_Product_ProductId",
                table: "ProductsComment");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductsComment",
                newName: "ProductsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductsComment_ProductId",
                table: "ProductsComment",
                newName: "IX_ProductsComment_ProductsId");

            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "Product",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Pictures",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsComment_Product_ProductsId",
                table: "ProductsComment",
                column: "ProductsId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
