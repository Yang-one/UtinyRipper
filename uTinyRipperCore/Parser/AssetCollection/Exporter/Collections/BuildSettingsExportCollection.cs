﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using uTinyRipper.Classes;
using uTinyRipper.Classes.EditorBuildSettingss;
using uTinyRipper.SerializedFiles;

namespace uTinyRipper.AssetExporters
{
	public sealed class BuildSettingsExportCollection : ManagerExportCollection
	{
		public BuildSettingsExportCollection(IAssetExporter assetExporter, VirtualSerializedFile file, Object asset) :
			this(assetExporter, file, (BuildSettings)asset)
		{
		}

		public BuildSettingsExportCollection(IAssetExporter assetExporter, VirtualSerializedFile virtualFile, BuildSettings asset) :
			base(assetExporter, asset)
		{
			EditorBuildSettings = EditorBuildSettings.CreateVirtualInstance(virtualFile);
			EditorSettings = EditorSettings.CreateVirtualInstance(virtualFile);
			if (!NavMeshProjectSettings.IsReadNavMeshProjectSettings(asset.File.Version))
			{
				NavMeshProjectSettings = NavMeshProjectSettings.CreateVirtualInstance(virtualFile);
			}
			if (!NetworkManager.IsReadNetworkManager(asset.File.Version))
			{
				NetworkManager = NetworkManager.CreateVirtualInstance(virtualFile);
			}
			if (!Physics2DSettings.IsReadPhysics2DSettings(asset.File.Version))
			{
				Physics2DSettings = Physics2DSettings.CreateVirtualInstance(virtualFile);
			}
			if (!UnityConnectSettings.IsReadUnityConnectSettings(asset.File.Version))
			{
				UnityConnectSettings = UnityConnectSettings.CreateVirtualInstance(virtualFile);
			}
			if (!QualitySettings.IsReadQualitySettings(asset.File.Version))
			{
				QualitySettings = QualitySettings.CreateVirtualInstance(virtualFile);
			}
		}

		public override bool Export(ProjectAssetContainer container, string dirPath)
		{
			string subPath = Path.Combine(dirPath, ProjectSettingsName);
			string fileName = $"{EditorBuildSettings.ClassID.ToString()}.asset";
			string filePath = Path.Combine(subPath, fileName);

			if (!DirectoryUtils.Exists(subPath))
			{
				DirectoryUtils.CreateDirectory(subPath);
			}

			BuildSettings asset = (BuildSettings)Asset;
			IEnumerable<Scene> scenes = asset.Scenes.Select(t => new Scene(t, container.SceneNameToGUID(t)));
			EditorBuildSettings.Initialize(scenes);
			AssetExporter.Export(container, EditorBuildSettings, filePath);

			fileName = $"{EditorSettings.ClassID.ToString()}.asset";
			filePath = Path.Combine(subPath, fileName);

			AssetExporter.Export(container, EditorSettings, filePath);

			if (NavMeshProjectSettings != null)
			{
				fileName = $"{NavMeshProjectSettings.ExportName}.asset";
				filePath = Path.Combine(subPath, fileName);

				AssetExporter.Export(container, NavMeshProjectSettings, filePath);
			}
			if (NetworkManager != null)
			{
				fileName = $"{NetworkManager.ExportName}.asset";
				filePath = Path.Combine(subPath, fileName);

				AssetExporter.Export(container, NetworkManager, filePath);
			}
			if (Physics2DSettings != null)
			{
				fileName = $"{Physics2DSettings.ExportName}.asset";
				filePath = Path.Combine(subPath, fileName);

				AssetExporter.Export(container, Physics2DSettings, filePath);
			}
			if (UnityConnectSettings != null)
			{
				fileName = $"{UnityConnectSettings.ExportName}.asset";
				filePath = Path.Combine(subPath, fileName);

				AssetExporter.Export(container, UnityConnectSettings, filePath);
			}
			if (QualitySettings != null)
			{
				fileName = $"{QualitySettings.ExportName}.asset";
				filePath = Path.Combine(subPath, fileName);

				AssetExporter.Export(container, QualitySettings, filePath);
			}

			fileName = $"ProjectVersion.txt";
			filePath = Path.Combine(subPath, fileName);

			using (FileStream file = FileUtils.Create(filePath))
			{
				using (StreamWriter writer = new InvariantStreamWriter(file, new UTF8Encoding(false)))
				{
					writer.Write("m_EditorVersion: 2017.3.0f3");
				}
			}
			return true;
		}

		public override bool IsContains(Object asset)
		{
			if (asset == EditorBuildSettings || asset == EditorSettings || asset == NavMeshProjectSettings || asset == NetworkManager||
				asset == Physics2DSettings || asset == UnityConnectSettings || asset == QualitySettings)
			{
				return true;
			}
			return base.IsContains(asset);
		}

		public override long GetExportID(Object asset)
		{
			return 1;
		}

		public override IEnumerable<Object> Assets
		{
			get
			{
				yield return Asset;
				yield return EditorBuildSettings;
				yield return EditorSettings;
				if (NavMeshProjectSettings != null)
				{
					yield return NavMeshProjectSettings;
				}
				if (NetworkManager != null)
				{
					yield return NetworkManager;
				}
				if(Physics2DSettings != null)
				{
					yield return Physics2DSettings;
				}
				if (UnityConnectSettings != null)
				{
					yield return UnityConnectSettings;
				}
				if (QualitySettings != null)
				{
					yield return QualitySettings;
				}
			}
		}

		public EditorBuildSettings EditorBuildSettings { get; }
		public EditorSettings EditorSettings { get; }
		public NavMeshProjectSettings NavMeshProjectSettings { get; }
		public NetworkManager NetworkManager { get; }
		public Physics2DSettings Physics2DSettings { get; }
		public UnityConnectSettings UnityConnectSettings { get; }
		public QualitySettings QualitySettings { get; }
	}
}
