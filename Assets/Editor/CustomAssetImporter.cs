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
			if (!assetPath.Contains("Sprites")) return;
			var importer = assetImporter as TextureImporter;
			ImportSprite(importer);
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
	}
}