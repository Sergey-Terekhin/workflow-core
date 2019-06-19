using Microsoft.EntityFrameworkCore.Migrations;
using System;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.SqlServer.Migrations
{
    public partial class StepScope : Migration
    {
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: false,
                defaultValue: 0);           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");
            
        }
    }
}
