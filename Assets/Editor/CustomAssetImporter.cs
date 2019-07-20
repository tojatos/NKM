using UnityEditor;
using UnityEngine;

namespace Editor
{
	/// <summary>
	/// Setting settings automatically after importing a sprite
	/// </summary>
	internal sealed class CustomAssetImporter : AssetPostprocessor
	{
		private void OnPreprocessTexture()
		{
			var importer = assetImporter as TextureImporter;
			if (assetPath.Contains("Sprites"))
			{
				ImportSprite(importer);
			}
			if (assetPath.Contains("Maps"))
			{
				ImportMap(importer);
			}
		}

		private static void ImportSprite(TextureImporter importer)
		{
			importer.textureType = TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.textureShape = TextureImporterShape.Texture2D;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.mipmapEnabled = false;
			importer.alphaIsTransparency = importer.DoesSourceTextureHaveAlpha();
		}
		
		private static void ImportMap(TextureImporter importer)
		{
			importer.isReadable = true;
			importer.filterMode = FilterMode.Point;
			importer.spritePixelsPerUnit = 1;
		}
	}
}