namespace FolderCompressAndEncrypt.Models.Enums
{
    /// <summary>
    /// Compression level of resulting archive. None / lower will take less time but result in largeer file sizes, 
    /// higher compression takes longer but results in small file sizes.
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// Uses 'store' (no algorithm) mode.
        /// </summary>
        None,

        /// <summary>
        /// Uses deflate algorithm. We're not using Lzma2 simply to give the option for compatibility purposes.
        /// </summary>
        Low,

        /// <summary>
        /// Uses deflate algorithm. We're not using Lzma2 simply to give the option for compatibility purposes.
        /// </summary>
        Normal,

        /// <summary>
        /// Uses Lzma2 algorithm.
        /// </summary>
        High,

        /// <summary>
        /// Uses Lzma2 algorithm.
        /// </summary>
        Ultra
    }
}
