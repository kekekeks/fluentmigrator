﻿using System.Data;
using FluentMigrator.Exceptions;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Jet;
using NUnit.Framework;
using NUnit.Should;

namespace FluentMigrator.Tests.Unit.Generators.Jet
{
    public class JetAlterTableTests : BaseTableAlterTests
    {
        //ALTER TABLE table
        //{ADD {COLUMN field type[(size)] [NOT NULL] [CONSTRAINT index] |
        //CONSTRAINT multifieldindex} |
        //DROP {COLUMN field | CONSTRAINT indexname} }

        private JetGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new JetGenerator();
        }

        [Test]
        public override void CanAddColumn()
        {
            var expression = GeneratorTestHelper.GetCreateColumnExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD COLUMN [TestColumn1] VARCHAR(5) NOT NULL");
        }

        [Test]
        public override void CanAddDecimalColumn()
        {
            var expression = GeneratorTestHelper.GetCreateDecimalColumnExpression();

            var result = _generator.Generate(expression);

            result.ShouldBe("ALTER TABLE [TestTable1] ADD COLUMN [TestColumn1] DECIMAL(19,2) NOT NULL");
        }

        [Test]
        public override void CanCreateForeignKey()
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "ALTER TABLE [TestTable1] ADD CONSTRAINT [FK_Test] FOREIGN KEY ([TestColumn1]) REFERENCES [TestTable2] ([TestColumn2])");
        }

        [TestCase(Rule.SetDefault, "SET DEFAULT"), TestCase(Rule.SetNull, "SET NULL"), TestCase(Rule.Cascade, "CASCADE")]
        public void CanCreateForeignKeyWithOnUpdateOptions(Rule rule, string output) 
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();
            expression.ForeignKey.OnUpdate = rule;
            var sql = _generator.Generate(expression);
            sql.ShouldBe(
                string.Format("ALTER TABLE [TestTable1] ADD CONSTRAINT [FK_Test] FOREIGN KEY ([TestColumn1]) REFERENCES [TestTable2] ([TestColumn2]) ON UPDATE {0}", output));
        }

        [TestCase(Rule.SetDefault, "SET DEFAULT"), TestCase(Rule.SetNull, "SET NULL"), TestCase(Rule.Cascade, "CASCADE")]
        public void CanCreateForeignKeyWithOnDeleteOptions(Rule rule, string output) 
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();
            expression.ForeignKey.OnDelete = rule;
            var sql = _generator.Generate(expression);
            sql.ShouldBe(
                string.Format("ALTER TABLE [TestTable1] ADD CONSTRAINT [FK_Test] FOREIGN KEY ([TestColumn1]) REFERENCES [TestTable2] ([TestColumn2]) ON DELETE {0}", output));
        }

        [Test]
        public void CanCreateForeignKeyWithOnDeleteAndOnUpdateOptions() 
        {
            var expression = GeneratorTestHelper.GetCreateForeignKeyExpression();
            expression.ForeignKey.OnDelete = Rule.Cascade;
            expression.ForeignKey.OnUpdate = Rule.SetDefault;
            var sql = _generator.Generate(expression);
            sql.ShouldBe(
                "ALTER TABLE [TestTable1] ADD CONSTRAINT [FK_Test] FOREIGN KEY ([TestColumn1]) REFERENCES [TestTable2] ([TestColumn2]) ON DELETE CASCADE ON UPDATE SET DEFAULT");
        }

        [Test]
        public override void CanCreateMulitColumnForeignKey()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnForeignKeyExpression();

            var result = _generator.Generate(expression);
            result.ShouldBe(
                "ALTER TABLE [TestTable1] ADD CONSTRAINT [FK_Test] FOREIGN KEY ([TestColumn1], [TestColumn3]) REFERENCES [TestTable2] ([TestColumn2], [TestColumn4])");
        }

        [Test]
        public override void CanRenameColumn()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        [Test]
        public void CanRenameColumnInStrictMode()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            _generator.compatabilityMode = Runner.CompatabilityMode.STRICT;
            Assert.Throws<DatabaseOperationNotSupportedException>(() => _generator.Generate(expression));
        }

        [Test]
        public override void CanRenameTable()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        [Test]
        public void CanRenameTableInStrictMode()
        {
            var expression = GeneratorTestHelper.GetRenameColumnExpression();
            _generator.compatabilityMode = Runner.CompatabilityMode.STRICT;
            Assert.Throws<DatabaseOperationNotSupportedException>(() => _generator.Generate(expression));
        }

        [Test]
        public override void CanAlterColumn()
        {
            var expression = GeneratorTestHelper.GetAlterColumnExpression();

            var sql = _generator.Generate(expression);

            sql.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] VARCHAR(20) NOT NULL");
        }

        [Test]
        public override void CanCreateAutoIncrementColumn()
        {
            var expression = GeneratorTestHelper.GetAlterColumnAddAutoIncrementExpression();

            var sql = _generator.Generate(expression);

            sql.ShouldBe("ALTER TABLE [TestTable1] ALTER COLUMN [TestColumn1] COUNTER NOT NULL");
        }

        [Test]
        public override void CanAlterSchema()
        {
            var expression = new AlterSchemaExpression();
            var result = _generator.Generate(expression);
            result.ShouldBe(string.Empty);
        }

        [Test]
        public void CanAlterSchemaInStrictMode()
        {
            _generator.compatabilityMode = Runner.CompatabilityMode.STRICT;
            Assert.Throws<DatabaseOperationNotSupportedException>(() => _generator.Generate(new CreateSchemaExpression()));
        }

        [Test]
        public void CanCreateMultiColmnNamedUniqueConstraint()
        {
            var expression = GeneratorTestHelper.GetCreateMultiColumnNamedUniqueConstraintExpression();
            var result = _generator.Generate(expression);
            result.ShouldBe("ALTER TABLE [TestTable1] ADD CONSTRAINT [TESTUNIQUECONSTRAINT] UNIQUE ([TestColumn1], [TestColumn2])");
        }
    }
}
