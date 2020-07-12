using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Coiner.Business.Migrations
{
    public partial class AddBlockchainFieldsToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockChainAddress",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlockChainPublicKey",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockChainAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BlockChainPublicKey",
                table: "Users");
        }
    }
}
