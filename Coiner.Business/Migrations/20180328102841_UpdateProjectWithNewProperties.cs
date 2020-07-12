using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Coiner.Business.Migrations
{
    public partial class UpdateProjectWithNewProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjectPeriod",
                table: "Projects",
                newName: "FundraisingPeriod");

            migrationBuilder.RenameColumn(
                name: "ActivityField",
                table: "Projects",
                newName: "ActivityType");
            
            migrationBuilder.AddColumn<int>(
                name: "Product_SalesPercepective",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPlan",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Career_EngagementYears",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentageAsset",
                table: "Projects",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProjectAddress",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectPresentation",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Society_CreationDate",
                table: "Projects",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Society_LegaleIdentification",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Society_Name",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Society_StructureType",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebLink",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product_SalesPercepective",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BusinessPlan",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Career_EngagementYears",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PercentageAsset",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectAddress",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectPresentation",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Society_CreationDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Society_LegaleIdentification",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Society_Name",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Society_StructureType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WebLink",
                table: "Projects");
            
            migrationBuilder.RenameColumn(
                name: "FundraisingPeriod",
                table: "Projects",
                newName: "ProjectPeriod");

            migrationBuilder.RenameColumn(
                name: "ActivityType",
                table: "Projects",
                newName: "ActivityField");
        }
    }
}
