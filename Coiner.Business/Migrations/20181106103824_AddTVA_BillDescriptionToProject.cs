using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Coiner.Business.Migrations
{
    public partial class AddTVA_BillDescriptionToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Product_BillDescription",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Product_TVA",
                table: "Projects",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product_BillDescription",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Product_TVA",
                table: "Projects");
        }
    }
}
