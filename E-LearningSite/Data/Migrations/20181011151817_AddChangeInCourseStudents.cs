using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace E_LearningSite.Data.Migrations
{
    public partial class AddChangeInCourseStudents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudents_Courses_CourseId",
                table: "CourseStudents");

            migrationBuilder.DropIndex(
                name: "IX_CourseStudents_CourseId",
                table: "CourseStudents");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "CourseStudents",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "CourseStudents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_CourseId1",
                table: "CourseStudents",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudents_Courses_CourseId1",
                table: "CourseStudents",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseStudents_Courses_CourseId1",
                table: "CourseStudents");

            migrationBuilder.DropIndex(
                name: "IX_CourseStudents_CourseId1",
                table: "CourseStudents");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "CourseStudents");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "CourseStudents",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_CourseId",
                table: "CourseStudents",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseStudents_Courses_CourseId",
                table: "CourseStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
