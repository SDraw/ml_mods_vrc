using System.IO;
using System.IO.MemoryMappedFiles;

namespace ml_vsf_wrapper
{
    class MemoryMapWritter
    {
        MemoryMappedFile m_file = null;
        MemoryMappedViewStream m_stream = null;
        int m_dataSize = 0;

        public bool Open(string p_path, int p_dataSize = 1024)
        {
            if(m_file == null)
            {
                m_dataSize = p_dataSize;

                m_file = MemoryMappedFile.CreateOrOpen(p_path, m_dataSize, MemoryMappedFileAccess.ReadWrite);
                m_stream = m_file.CreateViewStream(0, m_dataSize, MemoryMappedFileAccess.ReadWrite);
            }
            return (m_file != null);
        }

        public bool Write(byte[] p_data)
        {
            bool l_result = false;
            if(m_stream != null)
            {
                try
                {
                    m_stream.Seek(0, SeekOrigin.Begin);
                    m_stream.Write(p_data, 0, (p_data.Length > m_dataSize) ? m_dataSize : p_data.Length);
                    m_stream.Flush();
                    l_result = true;
                }
                catch(System.Exception) { }
            }
            return l_result;
        }

        public void Close()
        {
            if(m_file != null)
            {
                m_stream.Close();
                m_stream.Dispose();
                m_stream = null;

                m_file.Dispose();
                m_file = null;

                m_dataSize = 0;
            }
        }
    }
}
