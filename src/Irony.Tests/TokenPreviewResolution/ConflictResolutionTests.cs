using Irony.Parsing;
using System.Linq;
using Xunit;

namespace Irony.Tests.TokenPreviewResolution
{

    public class ConflictResolutionTests {

    // samples to be parsed
    const string FieldSample = "private int SomeField;";
    const string PropertySample = "public string Name {}";
    const string FieldListSample = "private int Field1; public string Field2;";
  const string MixedListSample = @"
      public int Size {}
      private string TableName;
      override void Run()
      {
      }";

    // Full grammar, no hints - expect errors ---------------------------------------------------------------------
    [Fact]
    public void TestConflictGrammarNoHints_HasErrors() {
      var grammar = new ConflictGrammarNoHints();
      var parser = new Parser(grammar);
      Assert.True(parser.Language.Errors.Count > 0);
      //Cannot parse mixed list
      var sample = MixedListSample;
      var tree = parser.Parse(sample);
      Assert.NotNull(tree);
      Assert.True(tree.HasErrors());
    }

    // Hints in Rules --------------------------------------------------------------------------
    [Fact]
    public void TestConflictGrammarWithHintsOnRules() {
      var grammar = new ConflictGrammarWithHintsInRules();
      var parser = new Parser(grammar);
      Assert.True(parser.Language.Errors.Count == 0);
      // Field sample
      var sample = FieldSample;
      var tree = parser.Parse(sample);
      Assert.NotNull(tree);
      Assert.False(tree.HasErrors());

      Assert.NotNull(tree.Root);
      var term = tree.Root.Term as NonTerminal;
      Assert.NotNull(term);
      Assert.Equal("definition", term.Name);

      Assert.Single(tree.Root.ChildNodes);
      var modNode = tree.Root.ChildNodes[0].ChildNodes[0];
      Assert.Equal("fieldModifier", modNode.Term.Name);

      //Property 
      sample = PropertySample;
      tree = parser.Parse(sample);
      Assert.NotNull(tree);
      Assert.False(tree.HasErrors());

      Assert.NotNull(tree.Root);
      term = tree.Root.Term as NonTerminal;
      Assert.NotNull(term);
      Assert.Equal("definition", term.Name);

      Assert.Single(tree.Root.ChildNodes);
      modNode = tree.Root.ChildNodes[0].ChildNodes[0];
      Assert.Equal("propModifier", modNode.Term.Name);
    }

    //Hints on terms ---------------------------------------------------------------------
    [Fact]
    public void TestConflictGrammar_HintsOnTerms() {
      var grammar = new ConflictGrammarWithHintsOnTerms();
      var parser = new Parser(grammar);
      Assert.True(parser.Language.Errors.Count == 0);

      //Field list sample
      var sample = FieldListSample;
      var tree = parser.Parse(sample);
      Assert.NotNull(tree);
      Assert.False(tree.HasErrors());

      Assert.NotNull(tree.Root);
      var term = tree.Root.Term as NonTerminal;
      Assert.NotNull(term);
      Assert.Equal("StatementList", term.Name);

      Assert.Equal(2, tree.Root.ChildNodes.Count);
      var nodes = tree.Root.ChildNodes.Select(t => t.ChildNodes[0]).ToArray();
      Assert.Equal("fieldDef", nodes[0].Term.Name);
      Assert.Equal("fieldDef", nodes[1].Term.Name);

      //Mixed sample
      sample = MixedListSample;
      tree = parser.Parse(sample);
      Assert.NotNull(tree);
      Assert.False(tree.HasErrors());

      Assert.NotNull(tree.Root);
      term = tree.Root.Term as NonTerminal;
      Assert.NotNull(term);
      Assert.Equal("StatementList", term.Name);

      Assert.Equal(3, tree.Root.ChildNodes.Count);
      nodes = tree.Root.ChildNodes.Select(t => t.ChildNodes[0]).ToArray();
      Assert.Equal("propDef", nodes[0].Term.Name);
      Assert.Equal("fieldDef", nodes[1].Term.Name);
      Assert.Equal("methodDef", nodes[2].Term.Name);
    }

  }
}
