using System;

using PixelFormat = SharpDX.WIC.PixelFormat;

namespace GameOverlay.Drawing
{
    internal static class ImagePixelFormats
    {
        // PixelFormat sorted in a best compatibility and best color accuracy order
        private static readonly Guid[] _bestPixelFormats = new Guid[]
        {
            PixelFormat.Format144bpp8ChannelsAlpha,
            PixelFormat.Format128bpp7ChannelsAlpha,
            PixelFormat.Format128bppRGBAFloat,
            PixelFormat.Format128bppPRGBAFloat,
            PixelFormat.Format128bppRGBAFixedPoint,
            PixelFormat.Format128bppRGBFloat,
            PixelFormat.Format128bppRGBFixedPoint,
            PixelFormat.Format128bpp8Channels,
            PixelFormat.Format112bpp6ChannelsAlpha,
            PixelFormat.Format112bpp7Channels,
            PixelFormat.Format96bppRGBFloat,
            PixelFormat.Format96bppRGBFixedPoint,
            PixelFormat.Format96bpp5ChannelsAlpha,
            PixelFormat.Format96bpp6Channels,
            PixelFormat.Format80bpp4ChannelsAlpha,
            PixelFormat.Format80bppCMYKAlpha,
            PixelFormat.Format80bpp5Channels,
            PixelFormat.Format72bpp8ChannelsAlpha,
            PixelFormat.Format64bppPBGRA,
            PixelFormat.Format64bppPRGBA,
            PixelFormat.Format64bppBGRA,
            PixelFormat.Format64bppRGBA,
            PixelFormat.Format64bppRGB,
            PixelFormat.Format64bppRGBFixedPoint,
            PixelFormat.Format64bppRGBHalf,
            PixelFormat.Format64bppBGRAFixedPoint,
            PixelFormat.Format64bppRGBAFixedPoint,
            PixelFormat.Format64bppRGBAHalf,
            PixelFormat.Format64bppPRGBAHalf,
            PixelFormat.Format64bpp7ChannelsAlpha,
            PixelFormat.Format64bpp3ChannelsAlpha,
            PixelFormat.Format64bppCMYK,
            PixelFormat.Format64bpp8Channels,
            PixelFormat.Format64bpp4Channels,
            PixelFormat.Format56bpp6ChannelsAlpha,
            PixelFormat.Format56bpp7Channels,
            PixelFormat.Format48bppBGR,
            PixelFormat.Format48bppBGRFixedPoint,
            PixelFormat.Format48bppRGB,
            PixelFormat.Format48bppRGBFixedPoint,
            PixelFormat.Format48bppRGBHalf,
            PixelFormat.Format48bpp5ChannelsAlpha,
            PixelFormat.Format48bpp6Channels,
            PixelFormat.Format48bpp3Channels,
            PixelFormat.Format40bpp4ChannelsAlpha,
            PixelFormat.Format40bppCMYKAlpha,
            PixelFormat.Format40bpp5Channels,
            PixelFormat.Format32bpp3ChannelsAlpha,
            PixelFormat.Format32bppRGBA,
            PixelFormat.Format32bppBGRA,
            PixelFormat.Format32bppPRGBA,
            PixelFormat.Format32bppPBGRA,
            PixelFormat.Format32bppRGBA1010102XR,
            PixelFormat.Format32bppRGBA1010102,
            PixelFormat.Format32bppBGR,
            PixelFormat.Format32bppRGB,
            PixelFormat.Format32bppRGBE,
            PixelFormat.Format32bppBGR101010,
            PixelFormat.Format32bpp4Channels,
            PixelFormat.Format32bppGrayFloat,
            PixelFormat.Format32bppCMYK,
            PixelFormat.Format32bppGrayFixedPoint,
            PixelFormat.Format24bppBGR,
            PixelFormat.Format24bppRGB,
            PixelFormat.Format24bpp3Channels,
            PixelFormat.Format16bppBGRA5551,
            PixelFormat.Format16bppBGR565,
            PixelFormat.Format16bppBGR555,
            PixelFormat.Format16bppCrQuantizedDctCoefficients,
            PixelFormat.Format16bppCbQuantizedDctCoefficients,
            PixelFormat.Format16bppYQuantizedDctCoefficients,
            PixelFormat.Format16bppCbCr,
            PixelFormat.Format16bppGray,
            PixelFormat.Format16bppGrayFixedPoint,
            PixelFormat.Format16bppGrayHalf,
            PixelFormat.Format8bppAlpha,
            PixelFormat.Format8bppCr,
            PixelFormat.Format8bppCb,
            PixelFormat.Format8bppY,
            PixelFormat.Format8bppGray,
            PixelFormat.Format8bppIndexed,
            PixelFormat.Format4bppGray,
            PixelFormat.Format4bppIndexed,
            PixelFormat.Format2bppGray,
            PixelFormat.Format2bppIndexed,
            PixelFormat.Format1bppIndexed,
            PixelFormat.FormatBlackWhite,
            PixelFormat.FormatDontCare
        };

        private static readonly Guid[] _mostCommonPixelFormats = new Guid[]
        {
            PixelFormat.Format32bppRGBA,
            PixelFormat.Format32bppBGRA,
            PixelFormat.Format32bppPRGBA,
            PixelFormat.Format32bppPBGRA,
            PixelFormat.Format32bppBGR,
            PixelFormat.Format32bppRGB,
            PixelFormat.Format24bppBGR,
            PixelFormat.Format24bppRGB,
            PixelFormat.Format24bpp3Channels,
            PixelFormat.Format32bpp4Channels,
            PixelFormat.Format32bppRGBE,
            PixelFormat.Format32bppBGR101010,
            PixelFormat.Format32bppRGBA1010102XR,
            PixelFormat.Format32bppRGBA1010102,
            PixelFormat.Format32bpp3ChannelsAlpha,
            PixelFormat.Format32bppGrayFloat,
            PixelFormat.Format32bppCMYK,
            PixelFormat.Format32bppGrayFixedPoint,
        };

        public static Guid GetBestPixelFormat(Guid[] supportedFormats)
        {
            if (supportedFormats == null || supportedFormats.Length == 0)
            {
                return PixelFormat.Format32bppPRGBA;
            }
            else
            {
                for (int i = 0; i < _mostCommonPixelFormats.Length; i++)
                {
                    for (int x = 0; x < supportedFormats.Length; x++)
                    {
                        if (_mostCommonPixelFormats[i] == supportedFormats[x]) return supportedFormats[x];
                    }
                }

                for (int i = 0; i < _bestPixelFormats.Length; i++)
                {
                    for (int x = 0; x < supportedFormats.Length; x++)
                    {
                        if (_bestPixelFormats[i] == supportedFormats[x]) return supportedFormats[x];
                    }
                }

                return supportedFormats[0];
            }
        }
    }
}
