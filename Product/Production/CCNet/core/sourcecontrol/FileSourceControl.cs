using System;
using System.Collections;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
	[ReflectorType("filesystem")]
	public class FileSourceControl : ISourceControl
	{
		private readonly IFileSystem fileSystem;

		public FileSourceControl() : this(new SystemIoFileSystem())
		{}

		public FileSourceControl(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		[ReflectorProperty("repositoryRoot")]
		public string RepositoryRoot;

		[ReflectorProperty("ignoreMissingRoot", Required=false)]
		public bool IgnoreMissingRoot;

		[ReflectorProperty("autoGetSource", Required = false)]
		public bool AutoGetSource = false;

		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			DirectoryInfo root = new DirectoryInfo(RepositoryRoot);
			ArrayList modifications = GetMods(root, from.StartTime);
			return (Modification[]) modifications.ToArray(typeof (Modification));
		}

		private ArrayList GetMods(DirectoryInfo dir, DateTime from)
		{
			ArrayList mods = new ArrayList();
			try
			{
				foreach (FileInfo file in dir.GetFiles())
				{
					if (IsLocalFileChanged(file, from))
					{
						mods.Add(CreateModification(file));
					}
				}

				foreach (DirectoryInfo sub in dir.GetDirectories())
				{
					mods.AddRange(GetMods(sub, from));
				}
			}
			catch (DirectoryNotFoundException exc)
			{
				if (!IgnoreMissingRoot)
				{
					throw exc;
				}
			}

			return mods;
		}

		private Modification CreateModification(FileInfo info)
		{
			Modification modification = new Modification();
			modification.FileName = info.Name;
			modification.ModifiedTime = info.LastWriteTime;
			modification.FolderName = info.DirectoryName;
			return modification;
		}

		private bool IsLocalFileChanged(FileInfo reposFile, DateTime date)
		{
			return reposFile.LastWriteTime > date;
		}

		public void LabelSourceControl(IIntegrationResult result)
		{}

		public void GetSource(IIntegrationResult result)
		{
			if (AutoGetSource)
				fileSystem.Copy(RepositoryRoot, result.WorkingDirectory);
		}

		public void Initialize(IProject project)
		{}

		public void Purge(IProject project)
		{}
	}
}