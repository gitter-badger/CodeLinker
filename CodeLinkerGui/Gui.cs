﻿// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace CodeLinker
{
  public partial class Gui : Form
  {
    public BindingSource source;

    public Gui()
    {
      InitializeComponent();
      source = new BindingSource(projectsList, null);
      projectListDataGridView.DataSource = source;
      projectListDataGridView.AutoGenerateColumns = true;
      projectListDataGridView.Columns[0].FillWeight = 5;
      projectListDataGridView.Columns[1].FillWeight = 2;
      source.AddingNew += OnAddingNewToBindingSource;
    }


    private BindingList<ProjectToLink> projectsList = new BindingList<ProjectToLink>();


    private void projectListDataGridView_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
        ? DragDropEffects.All
        : DragDropEffects.None;
    }


    private void Sources_DragDrop(object sender, DragEventArgs e)
    {
      AddFilesOrFolderToSources((string[])e.Data.GetData(DataFormats.FileDrop, false));
    }


    private void AddFilesOrFolderToSources(string[] filesOrFolders)
    {
      if (projectListDataGridView.Rows.Count == source.Count)
      {
        source.RemoveAt(source.Count - 1);
      } // http://stackoverflow.com/a/27051109/492

      projectListDataGridView.Refresh();
      List<string> filepaths = new List<string>();
      foreach (string fileOrFolder in filesOrFolders)
      {
        if (Directory.Exists(fileOrFolder))
        {
          filepaths.AddRange(Directory.GetFiles(fileOrFolder, "*.??proj", SearchOption.AllDirectories));
          SourceProjectFolderTextBox.Text = fileOrFolder; // last one wins. May or may not be useful.
        }
        else if (fileOrFolder.IsaCsOrVbProjFile())
        {
          filepaths.Add(fileOrFolder);
        }
      }
      foreach (string filePath in filepaths)
      {
        if (!(projectsList.Any(p => p?.SourceProject == filePath)))
        {
          projectsList.Add(new ProjectToLink {SourceProject = filePath, DestinationProjectName = Path.GetFileName(filePath)});
        }
      }
      CheckProjectsList();
    }


    private void FolderTextBox_DragEnter(object sender, DragEventArgs e)
    {
      string[] drops = (string[])e.Data.GetData(DataFormats.FileDrop, false);
      if (drops.Any() && (Directory.Exists(drops[0])))
      {
        e.Effect = DragDropEffects.All;
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }

    private void SourceFolderTextBox_DragDrop(object sender, DragEventArgs e)
    {
      ProjectFolderTextBox_DragDrop(sender, e);
      Sources_DragDrop(sender, e);
    }

    private void ProjectFolderTextBox_DragDrop(object sender, DragEventArgs e)
    {
      string[] drops = (string[])e.Data.GetData(DataFormats.FileDrop, false);
      if (!drops.Any())
      {
        e.Effect = DragDropEffects.None;
        return;
      }
      if (Directory.Exists(drops[0]))
      {
        TextBox tb = (TextBox)sender;
        tb.Text = drops[0];
        tb.BackColor = SourceProjectFolderTextBox.BackColor;
      }
      CheckProjectsList();
    }


    private void SourceFolderButton_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowser = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.Desktop};

      if (!string.IsNullOrEmpty(SourceProjectFolderTextBox.Text) && Directory.Exists(SourceProjectFolderTextBox.Text))
      {
        folderBrowser.SelectedPath = SourceProjectFolderTextBox.Text;
      }

      folderBrowser.ShowNewFolderButton = true;

      if (folderBrowser.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }

      SourceProjectFolderTextBox.Text = folderBrowser.SelectedPath;
      AddFilesOrFolderToSources(new string[] {folderBrowser.SelectedPath});
    }


    private void DestinationFolderButton_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog folderBrowser = new FolderBrowserDialog {RootFolder = Environment.SpecialFolder.Desktop};

      if (!string.IsNullOrEmpty(DestinationProjectFolderTextBox.Text) && Directory.Exists(DestinationProjectFolderTextBox.Text))
      {
        folderBrowser.SelectedPath = DestinationProjectFolderTextBox.Text;
      }
      else if (!string.IsNullOrEmpty(SourceProjectFolderTextBox.Text) && Directory.Exists(SourceProjectFolderTextBox.Text))
      {
        folderBrowser.SelectedPath = SourceProjectFolderTextBox.Text;
      }
      folderBrowser.ShowNewFolderButton = true;

      if (folderBrowser.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      DestinationProjectFolderTextBox.Text = folderBrowser.SelectedPath;
      DestinationProjectFolderTextBox.BackColor = SourceProjectFolderTextBox.BackColor;
      CheckProjectsList();
    }


    private void CheckProjectsList(object sender = null, EventArgs e = null)
    {
      foreach (DataGridViewRow row in projectListDataGridView.Rows)
      {
        if (row.Cells[1]?.Value != null) // source
        {
          string pathToCheck = Path.Combine(DestinationProjectFolderTextBox.Text ?? "", row.Cells[0].Value.ToString());
          if (!File.Exists(pathToCheck))
          {
            row.Cells[0].Style.ForeColor = Color.OrangeRed;
            row.Cells[0].Style.Font = new Font(projectListDataGridView.RowTemplate.DefaultCellStyle.Font, FontStyle.Bold);
            row.Cells[0].ToolTipText += "Source Project does not exist on disk.";
          }
          else
          {
            row.Cells[1].Style.ForeColor = row.Cells[0].Style.ForeColor;
            row.Cells[1].Style.Font = new Font(projectListDataGridView.RowTemplate.DefaultCellStyle.Font, FontStyle.Regular);
            row.Cells[0].ToolTipText = "";
          }
        }

        if (row.Cells[1]?.Value != null) // destination
        {
          if (projectsList.Count(destination => destination.DestinationProjectName == row.Cells[1].Value.ToString()) > 1)
          {
            row.Cells[1].Style.BackColor = Color.BlanchedAlmond;
            row.Cells[1].ToolTipText = "Destination is listed more than once. ";
          }
          else
          {
            row.Cells[1].Style.BackColor = row.Cells[0].Style.BackColor;
            row.Cells[1].ToolTipText = "";
          }

          string pathToCheck = Path.Combine(DestinationProjectFolderTextBox.Text ?? "", row.Cells[1].Value.ToString());
          if (File.Exists(pathToCheck))
          {
            row.Cells[1].Style.ForeColor = Color.OrangeRed;
            row.Cells[1].Style.Font = new Font(projectListDataGridView.RowTemplate.DefaultCellStyle.Font, FontStyle.Bold);
            row.Cells[1].ToolTipText += "Destination Project already exists on disk.";
          }
          else
          {
            row.Cells[1].Style.ForeColor = row.Cells[0].Style.ForeColor;
            row.Cells[1].Style.Font = new Font(projectListDataGridView.RowTemplate.DefaultCellStyle.Font, FontStyle.Regular);
          }
        }
      }
      projectListDataGridView.Refresh();
    }


    private void CheckProjectsList(object sender, DataGridViewCellEventArgs e)
    {
      CheckProjectsList(sender, (EventArgs)e);
    }

    private void CheckProjectsList(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      CheckProjectsList(sender, (EventArgs)e);
    }

    private void linkButton_Click(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(DestinationProjectFolderTextBox.Text) || !Directory.Exists(DestinationProjectFolderTextBox.Text))
      {
        DestinationProjectFolderTextBox.BackColor = Color.BlanchedAlmond;
        return;
      }
      ProjectMaker.NewProject(projectsList.ToList(), DestinationProjectFolderTextBox.Text);
      CheckProjectsList();
    }


    private void OnAddingNewToBindingSource(object sender, AddingNewEventArgs e)
    {
      if (projectListDataGridView.Rows.Count == source.Count) // http://stackoverflow.com/a/2363918/492
      {
        source.RemoveAt(source.Count - 1);
      }
    }
  }
}
