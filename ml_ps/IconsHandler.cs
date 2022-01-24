using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ml_ps
{
    // Straight copy from Requi's code
    static class IconsHandler
    {
        static Dictionary<string, Sprite> ms_sprites = new Dictionary<string, Sprite>();
        static Dictionary<string, Texture2D> ms_textures = new Dictionary<string, Texture2D>();

        public static Sprite GetIcon(string p_name)
        {
            Sprite l_result = null;
            if(ms_sprites.ContainsKey(p_name))
            {
                l_result = ms_sprites[p_name];
            }
            else
            {
                var l_texture = GetTexture(p_name);
                var l_rect = new Rect(0.0f, 0.0f, l_texture.width, l_texture.height);
                var l_pivot = new Vector2(0.5f, 0.5f);
                var l_border = Vector4.zero;
                var l_sprite = Sprite.CreateSprite_Injected(l_texture, ref l_rect, ref l_pivot, 100.0f, 0, SpriteMeshType.Tight, ref l_border, false);
                l_sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                ms_sprites.Add(p_name, l_sprite);
                l_result = l_sprite;
            }
            return l_result;
        }

        static Texture2D GetTexture(string p_name)
        {
            Texture2D l_result = null;
            if(ms_textures.ContainsKey(p_name))
            {
                l_result = ms_textures[p_name];
            }
            else
            {
                var l_assembly = Assembly.GetExecutingAssembly();
                Stream l_readStream = l_assembly.GetManifestResourceStream(l_assembly.GetName().Name + ".icons." + p_name + ".png");
                MemoryStream l_data = new MemoryStream();
                l_readStream.CopyTo(l_data);

                Texture2D l_texture = new Texture2D(1, 1);
                ImageConversion.LoadImage(l_texture, l_data.ToArray());
                l_texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                l_texture.wrapMode = TextureWrapMode.Clamp;

                ms_textures.Add(p_name, l_texture);
                l_result = l_texture;
            }
            return l_result;
        }
    }
}
