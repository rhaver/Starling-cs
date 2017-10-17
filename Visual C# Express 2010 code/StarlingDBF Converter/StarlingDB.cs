using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;

using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

using StringUtils;

namespace StarlingDB
{

    #region DBF header

    /// <summary>
    /// Class representing a Starling database (.dbf) record field's structural information
    /// </summary>
    class StarlingDbfField
    {
        /// <summary>
        /// Enumerator for the different field types.
        /// </summary>
        public enum FieldType
        {
            character = 'C', currency = 'Y', numeric = 'N', @float = 'F', date = 'D', datetime = 'T', @double = 'B',
            integer = 'I', logical = 'L', memo = 'M', general = 'G', picture = 'P'
        };
        /// <summary>
        /// Enumerator for a field's bit flags.
        /// </summary>
        public enum FieldFlags
        {
            system_column = 0x01, nullable = 0x02, binary = 0x04
        }

        /// <summary>
        /// Field name.
        /// </summary>
        public readonly String name;
        /// <summary>
        /// Field type.
        /// </summary>
        public readonly FieldType type;
        /// <summary>
        /// Offset of the field, within the record. (Do not trust this value.)
        /// </summary>
        public readonly uint offset;
        /// <summary>
        /// Field length.
        /// </summary>
        public readonly byte fieldlength;
        /// <summary>
        /// Number of decimal places.
        /// </summary>
        public readonly byte decimals;
        /// <summary>
        /// (Combining) bit flags for the field.
        /// </summary>
        public readonly byte flags;
        /// <summary>
        /// Reserved bytes 19-31.
        /// </summary>
        public readonly byte[] reserved_19_31;

        public StarlingDbfField(byte[] rec)
        {
            if (rec.Length < 32)
                throw new ArgumentException("DBF record structure should be 32 bytes.");
            else
            {
                name = Encoding.ASCII.GetString(rec, 0, 11);
                name = name.TrimEnd('\0');
                type = (FieldType)rec[11];
                offset = BitConverter.ToUInt32(rec, 12);
                fieldlength = rec[16];
                decimals = rec[17];
                flags = rec[18];
                reserved_19_31 = new byte[13];
                Array.Copy(rec, 19, reserved_19_31, 0, reserved_19_31.Length);
            }
        }

        /// <summary>
        /// Show all values in a multiline string.
        /// </summary>
        /// <returns>Multiline string.</returns>
        public override string ToString()
        {
            return String.Format("Name:\t\t\t\t\t{0}\nType:\t\t\t\t\t{1}\nRecord offset:\t\t\t{2}\nLength:\t\t\t\t\t{3}\nDecimals\t\t\t\t{4}\n" +
                "Flags:\t\t\t\t\t{5}\nReserved bytes 24-31:\t{6}",
                name, type, offset, fieldlength, decimals, flags, BitConverter.ToString(reserved_19_31));
        }
    }

    /// <summary>
    /// Class for reading and representing a Starling database (.dbf) header.
    /// </summary>
    class StarlingDbfHeader
    {
        /// <summary>
        /// The constant value used to terminate a header record.
        /// </summary>
        public const byte HeaderRecordTerminator = 0x0D;
        /// <summary>
        /// Enumerates the possible .dbf file types.
        /// </summary>
        public enum DbfVersion { dBase3 = 0x3 };
        /// <summary>
        /// Enumerator for the database's bit flags.
        /// </summary>
        public enum DbfFlags { has_cdx = 0x1, has_memo = 0x2, is_dbc = 0x4 };

        /// <summary>
        /// .dbf filetype.
        /// </summary>
        public readonly byte dbf_filetype;
        /// <summary>
        /// Year of the last update (as an offset from 1900).
        /// </summary>
        public readonly byte last_update_year;
        /// <summary>
        /// Month of the last update.
        /// </summary>
        public readonly byte last_update_month;
        /// <summary>
        /// Day of the last update.
        /// </summary>
        public readonly byte last_update_day;
        /// <summary>
        /// Number of data records.
        /// </summary>
        public readonly uint NDR;
        /// <summary>
        /// Header size.
        /// </summary>
        public readonly ushort HSZ;
        /// <summary>
        /// Data record size.
        /// </summary>
        public readonly ushort DRS;
        /// <summary>
        /// Reserved bytes 12-27.
        /// </summary>
        public readonly byte[] reserved_12_27;
        /// <summary>
        /// (Combining) bit flags for the table.
        /// </summary>
        public readonly DbfFlags table_flags;
        /// <summary>
        /// Code page mark.
        /// </summary>
        public readonly byte code_page_mark;
        /// <summary>
        /// Reserved bytes 30-31.
        /// </summary>
        public readonly byte[] reserved_30_31;

        /// <summary>
        /// List of field structures.
        /// </summary>
        public readonly List<StarlingDbfField> field_structures;

        public StarlingDbfHeader(String DbfFilename)
        {
            using (BinaryReader dbfFile = new BinaryReader(File.OpenRead(DbfFilename), Encoding.ASCII))
            {
                // initialize header variables
                dbf_filetype = dbfFile.ReadByte();
                last_update_year = dbfFile.ReadByte();
                last_update_month = dbfFile.ReadByte();
                last_update_day = dbfFile.ReadByte();
                NDR = dbfFile.ReadUInt32();
                HSZ = dbfFile.ReadUInt16();
                DRS = dbfFile.ReadUInt16();
                reserved_12_27 = dbfFile.ReadBytes(16);
                table_flags = (DbfFlags)dbfFile.ReadByte();
                code_page_mark = dbfFile.ReadByte();
                reserved_30_31 = dbfFile.ReadBytes(2);

                // get record structures
                field_structures = new List<StarlingDbfField>();
                while (dbfFile.PeekChar() != HeaderRecordTerminator && dbfFile.BaseStream.Position < HSZ)
                    field_structures.Add(new StarlingDbfField(dbfFile.ReadBytes(32)));
            }
        }

        /// <summary>
        /// Based on header info, compute the expected file size of the Starling database (.dbf).
        /// </summary>
        /// <returns>Computed file size.</returns>
        public uint getFileSize()
        {
            return HSZ + NDR * DRS + 1;
        }

        /// <summary>
        /// Show all values in a multiline string.
        /// </summary>
        /// <returns>Multiline string.</returns>
        public override String ToString()
        {
            return String.Format("Version:\t\t\t\t{0}\nLast update:\t\t\t{1:D4}-{2:D2}-{3:D2}\nNumber of data records:\t{4}\n" +
                "Header size:\t\t\t{5}\nData record size:\t\t{6}\nReserved bytes 12-27:\t{7}\nFlags:\t\t\t\t\t{8}\n" +
                "Codepage mark:\t\t\t{9}\nReserved bytes 30-31:\t{10}",
                dbf_filetype, last_update_year + (last_update_year > 60 ? 1900 : 2000), last_update_month, last_update_day, NDR, HSZ, DRS, BitConverter.ToString(reserved_12_27), table_flags, code_page_mark, BitConverter.ToString(reserved_30_31));
        }
    }

    #endregion

    #region DBF record

    /// <summary>
    /// Class representing a Starling database (.dbf) record
    /// </summary>
    class StarlingDbfRecord
    {
        /// <summary>
        /// Reference to database header information, because it contains record structure.
        /// </summary>
        private StarlingDbfHeader header;

        /// <summary>
        /// Enumerator for a record's status.
        /// </summary>
        public enum @status { undeleted = 0x20, deleted = 0x2A};

        /// <summary>
        /// List of contents (as a byte array) for each field.
        /// </summary>
        public readonly List<byte[]> field_contents;
        
        /// <summary>
        /// Record status.
        /// </summary>
        public @status record_status;

        public StarlingDbfRecord(StarlingDbfHeader head, byte[] rec)
        {
            if (0 < rec.Length && rec.Length == head.DRS)
            {
                // keep reference to .dbf header
                header = head;
                // read record status
                record_status = (@status)rec[0];
                // read contents of all fields
                field_contents = new List<byte[]>();
                uint offset = 1;
                foreach (StarlingDbfField r in header.field_structures)
                {
                    byte[] ba = new byte[r.fieldlength];
                    Array.Copy(rec, offset, ba, 0, r.fieldlength);
                    field_contents.Add(ba);
                    offset += r.fieldlength;
                }
            }
            else
                throw new Exception("Wrong record size.");
        }

        /// <summary>
        /// Check if a field normally containing a reference to a position in the .var file is actually empty, i.e. filled with ASCII spaces.
        /// </summary>
        /// <param name="ba">The field contents as an array of bytes.</param>
        /// <returns>True if the field is empty, false otherwise.</returns>
        public static bool emptyVarRef(byte[] ba)
        {
            String bs = Encoding.ASCII.GetString(ba);
            if (bs.Trim() == String.Empty)
                return true;
            else
            {
                if (String.Equals(bs, "\n  \n\n ", StringComparison.Ordinal))
                {
                    // this case based on code by Andries Brouwer, but does this ever occur?
                    Trace.WriteLine("Encountered a \"↵␣␣↵↵␣\" sequence.");
                    return true; 
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Show all values in a multiline string.
        /// </summary>
        /// <returns>Multiline string.</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (record_status == status.deleted)
                sb.AppendLine("(deleted record)");
            else
                sb.AppendLine("(record)");
            for (int i = 0; i < header.field_structures.Count; i++)
            {
                sb.Append(String.Format("{0}:\t{1}\t{2}\n", header.field_structures[i].name.PadRight(12), Encoding.ASCII.GetString(field_contents[i]).PadRight(15), BitConverter.ToString(field_contents[i])));
            }
            sb.Append("\n");
            return sb.ToString();
        }
    }

    #endregion

    /// <summary>
    /// Class representing a Starling database (.dbf), providing functionality for opening and extracting data from it.
    /// </summary>
    class StarlingDbfReader
    {
        #region Progress reporting

        /// <summary>
        /// Delegate for handling a progress reporting event.
        /// </summary>
        /// <param name="progressPercent">Current progress, in percent.</param>
        /// <param name="message">A status message.</param>
        public delegate void ReportProgressEventHandler(double progressPercent, String message);

        /// <summary>
        /// Event that reports progress.
        /// </summary>
        public event ReportProgressEventHandler onProgressChanged;

        /// <summary>
        /// Helper function to report progress via an event.
        /// </summary>
        /// <param name="percent">Percentage in the range [0..100] of task completion to be reported.</param>
        /// <param name="status">A status message.</param>
        private void reportProgress(double percent, String status)
        {
            if (onProgressChanged != null)
                onProgressChanged(percent, status);
        }

        #endregion

        #region Class variables

        /// <summary>
        /// Database name, which is the name of the sourcefile without the extension.
        /// </summary>
        private String table_name;

        /// <summary>
        /// All records from the database, in their original order.
        /// </summary>
        private List<StarlingDbfRecord> records;

        /// <summary>
        /// The database header.
        /// </summary>
        public readonly StarlingDbfHeader header;

        #endregion

        #region Database conversion

        /// <summary>
        /// Helper function to map a Starling type to a C# .Net type.
        /// </summary>
        /// <param name="ft">The Starling type</param>
        /// <returns></returns>
        private static Type StarlingTypeToType(StarlingDbfField.FieldType ft)
        {
            Type t;
            switch (ft)
            {
                case StarlingDbfField.FieldType.integer:
                    t = typeof(int);
                    break;
                case StarlingDbfField.FieldType.@float:
                    t = typeof(float);
                    break;
                case StarlingDbfField.FieldType.numeric:
                case StarlingDbfField.FieldType.@double:
                    t = typeof(double);
                    break;
                case StarlingDbfField.FieldType.datetime:
                    t = typeof(DateTime);
                    break;
                case StarlingDbfField.FieldType.logical:
                    t = typeof(bool);
                    break;
                default:
                    t = typeof(String);
                    break;
            }
            return t;
        }

        /// <summary>
        /// Helper function to cast data extracted from a Starling database to data in a particular C# .Net type.
        /// </summary>
        /// <param name="s">The string with the decoded data from the database.</param>
        /// <param name="t">The type to cast it in.</param>
        /// <returns>An object of the desired type, representing the value of the original string.</returns>
        private Object decodedDataToType(String s, Type t)
        {
            switch (t.ToString())
            {
                case "System.DateTime":
                    DateTime dt;
                    DateTime.TryParse(s, out dt);
                    return dt;
                case "System.String":
                    return s;
                case "System.Boolean":
                    switch (s)
                    {
                        case "T":
                            return true;
                        case "":
                        case "F":
                            return false;
                        default:
                            try
                            {
                                return Convert.ToBoolean(s);
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(String.Format("Error converting to boolean from \"{0}\" ({1}). Value treated as 'false'.", s, e.Message));
                                return false;
                            }
                    }
                default:
                    // some numeric type (possibly a real number)
                    if (s == "")
                        return 0;
                    else
                    {
                        try
                        {
                            //String s_env = Regex.Replace(s, @"[.,]", NumberFormatInfo.InvariantInfo.NumberDecimalSeparator);
                            return Convert.ChangeType(s, t, CultureInfo.InvariantCulture);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(String.Format("Error converting to numeric from \"{0}\" ({1}). Value treated as 0.", s, e.Message));
                            return 0;
                        }
                    }
            }
        }

        /// <summary>
        /// Helper function that turns the list of records into a DataTable, based on a decoding function that turns the raw byte data into a String.
        /// </summary>
        /// <param name="decodeBytes">Function to decode an array of bytes to a string.</param>
        /// <param name="decodeField">Function to choose a C# .Net type for a DataColumn, based on a Starling field type.</param>
        /// <returns>DataTable populated with the database's data.</returns>
        private DataTable getDataTable(Func<byte[], String> decodeBytes, Func<StarlingDbfField.FieldType, Type> decodeField)
        {
            DataTable dt = new DataTable(table_name, "ns-" + table_name);

            // add a column for the deleted marker and populate it
            dt.Columns.Add("deleted", typeof(bool));
            foreach (StarlingDbfRecord rec in records)
                dt.Rows.Add(rec.record_status == StarlingDbfRecord.status.deleted);
            
            // add other columns
            int cell = 0;
            for (int index_field = 0; index_field < header.field_structures.Count; index_field++)
            {
                // add column
                StarlingDbfField fld = header.field_structures[index_field];
                Type coltype = decodeField(fld.type);
                DataColumn dc = new DataColumn(fld.name, coltype);
                dt.Columns.Add(dc);

                // populate column
                for (int index_rec = 0; index_rec < records.Count; index_rec++)
                {
                    // get database data as string
                    String s = decodeBytes(records[index_rec].field_contents[index_field]).Trim();
                    
                    // add cell, with the data in the appropriate type
                    dt.Rows[index_rec].SetField(dc, decodedDataToType(s, coltype));

                    // report decoding difficulties/progress
                    if (s.Contains(StarlingDecoder.fallbackCharacter.ToString()))
                        Trace.WriteLine(String.Format("Check record {0}, field {1}.", index_rec + 1, index_field + 2));
                    reportProgress((100d * ++cell) / (records.Count * header.field_structures.Count), "Decoding");
                }
            }

            return dt;
        }

        /// <summary>
        /// Function that gets the database as a DataTable filled with Strings (being ASCII representations of the raw byte data).
        /// </summary>
        /// <returns>DataTable populated with the database's data in String form.</returns>
        public DataTable getDataTableRawish()
        {
            // all data in Starling is kept as a String, in ASCII interpretation
            return getDataTable(Encoding.ASCII.GetString, x => typeof(String));
        }

        /// <summary>
        /// Function that gets the database as a DataTable filled with the decoded data, cast into types that match the original
        /// Starling type most closely.
        /// </summary>
        /// <returns>DataTable populated with the database's data.</returns>
        public DataTable getDataTableDecoded()
        {
            return getDataTable(StarlingDecoder.decode, StarlingTypeToType);
        }

        #endregion

        /// <summary>
        /// Constructor method that opens and reads a Starling database (.dbf).
        /// </summary>
        /// <param name="dbfFilename">Filename of the Starling database to be read.</param>
        /// <param name="progressHandler">Optional event handler for progress reports.</param>
        public StarlingDbfReader(String dbfFilename, ReportProgressEventHandler progressHandler = null)
        {
            // register optional event
            if (progressHandler != null)
                onProgressChanged += progressHandler;

            // get var filename
            String VarFilename = Path.ChangeExtension(dbfFilename, "var");
            table_name = Path.GetFileNameWithoutExtension(dbfFilename);

            if (File.Exists(dbfFilename))
            {
                // read header with database structure
                reportProgress(0, "Reading header");
                header = new StarlingDbfHeader(dbfFilename);
                reportProgress(10, "Reading header: fields");
                
                // log header info
                Trace.WriteLine(dbfFilename);
                Trace.WriteLine(String.Format("DBF Filesize:\t\t{0}", header.getFileSize()));
                Trace.WriteLine(String.Format("Actual filesize:\t{0}", new FileInfo(dbfFilename).Length));
                Trace.WriteLine("Header");
                Trace.Indent();
                foreach (String s in header.ToString().Split('\n'))
                    Trace.WriteLine(s);
                Trace.Unindent();
                Trace.WriteLine(String.Format("Fields ({0})", header.field_structures.Count));
                Trace.Indent();

                // read field structures
                for (int i = 0; i < header.field_structures.Count; i++)
                {
                    StarlingDbfField rec = header.field_structures[i];

                    // log field info
                    Trace.WriteLine(String.Format("Field {0}", i));
                    Trace.Indent();
                    foreach (String s in rec.ToString().Split('\n'))
                        Trace.WriteLine(s);
                    Trace.Unindent();

                    // report progress
                    reportProgress(10 + 10 * ((float)i / header.field_structures.Count), "Reading header: fields");
                }
                Trace.Unindent();

                // read byte content for all records
                reportProgress(20, "Reading records");
                records = new List<StarlingDbfRecord>();
                using (BinaryReader dbfFile = new BinaryReader(File.OpenRead(dbfFilename), Encoding.ASCII))
                {
                    dbfFile.ReadBytes(header.HSZ);
                    while (dbfFile.BaseStream.Length - dbfFile.BaseStream.Position >= header.DRS)
                    {
                        StarlingDbfRecord rec = new StarlingDbfRecord(header, dbfFile.ReadBytes(header.DRS));
                        records.Add(rec);
                        reportProgress(20 + (40d * dbfFile.BaseStream.Position) / dbfFile.BaseStream.Length, "Reading records");
                    }
                }


                if (File.Exists(VarFilename))
                {
                    // in case of an accompanying .var file, replace byte content of record fields if they are pointers to data in .var file
                    // first check if we are dealing with a .dbf file and if it contains data pointers
                    bool varfile_is_needed = false;
                    if (String.Equals(Path.GetExtension(dbfFilename), ".dbf", StringComparison.OrdinalIgnoreCase))
                    {
                        for (int i = 0; i < header.field_structures.Count && !varfile_is_needed; i++)
                        {
                            StarlingDbfField fld = header.field_structures[i];
                            varfile_is_needed = (fld.type == StarlingDbfField.FieldType.character && fld.fieldlength == 6);
                        }
                    }

                    if (varfile_is_needed)
                    {
                        reportProgress(60, "Processing records");
                        using (BinaryReader varFile = new BinaryReader(File.OpenRead(VarFilename), Encoding.ASCII))
                        {
                            // walk through all fields (columns)
                            for (int i = 0; i < header.field_structures.Count; i++)
                            {
                                // check if the field contains data pointers, if so update each record's contents
                                StarlingDbfField fld = header.field_structures[i];
                                if (fld.type == StarlingDbfField.FieldType.character && fld.fieldlength == 6)
                                {
                                    /*
                                     * We are dealing with a pointer to the VAR file here.
                                     * The first 4 bytes give the position, the last 2 bytes give length.
                                     */
                                    foreach (StarlingDbfRecord rec in records)
                                    {
                                        byte[] old_content = rec.field_contents[i];
                                        if (!StarlingDbfRecord.emptyVarRef(old_content))
                                        {
                                            uint position = BitConverter.ToUInt32(old_content, 0);
                                            ushort length = BitConverter.ToUInt16(old_content, 4);
                                            varFile.BaseStream.Seek(position, SeekOrigin.Begin);
                                            byte[] new_content = varFile.ReadBytes(length);
                                            rec.field_contents[i] = new_content;
                                        }
                                    }
                                }
                                reportProgress(60 + (40d * i) / header.field_structures.Count, "Processing records");
                            }
                        }
                    }
                }

                reportProgress(100, "Database loaded");

                // log records
                //foreach (StarlingDbfRecord rec in records)
                //    Trace.WriteLine(rec.ToString());
            }
            else
            {
                Trace.WriteLine("DBF file not found.");
            }

            // unregister event
            if (progressHandler != null)
                onProgressChanged -= progressHandler;
        }
    }
}
