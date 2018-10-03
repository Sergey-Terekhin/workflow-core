using System;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class WfReference : Migration
    {
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                schema: _schema,
                table: _tablePrefix + "Workflow",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                schema: _schema,
                table: _tablePrefix + "Workflow");
        }
    }
}
