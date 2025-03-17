using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace onlineshop.Migrations
{
    /// <inheritdoc />
    public partial class adding_product_in_T_basket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "T_Basket",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_T_Basket_ProductId",
                table: "T_Basket",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_T_Basket_T_Product_ProductId",
                table: "T_Basket",
                column: "ProductId",
                principalTable: "T_Product",
                principalColumn: "ID_Product",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_Basket_T_Product_ProductId",
                table: "T_Basket");

            migrationBuilder.DropIndex(
                name: "IX_T_Basket_ProductId",
                table: "T_Basket");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "T_Basket");
        }
    }
}
