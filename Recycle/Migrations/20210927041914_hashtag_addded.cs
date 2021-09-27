using Microsoft.EntityFrameworkCore.Migrations;

namespace Recycle.Migrations
{
    public partial class hashtag_addded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hashtag",
                columns: table => new
                {
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashtag", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "HashtagProduct",
                columns: table => new
                {
                    HashtagsTitle = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HashtagProduct", x => new { x.HashtagsTitle, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_HashtagProduct_Hashtag_HashtagsTitle",
                        column: x => x.HashtagsTitle,
                        principalTable: "Hashtag",
                        principalColumn: "Title",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HashtagProduct_Product_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HashtagProduct_ProductsId",
                table: "HashtagProduct",
                column: "ProductsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HashtagProduct");

            migrationBuilder.DropTable(
                name: "Hashtag");
        }
    }
}
