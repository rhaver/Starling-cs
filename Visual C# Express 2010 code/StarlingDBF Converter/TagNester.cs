using System;
using System.Collections.Generic;
using System.Text;

namespace TagFixer
{
    /// <summary>
    /// Class to represent a tag, for use by the class TagNester.
    /// </summary>
    class Tag
    {
        /// <summary>
        /// The number of bits available for the bit mask.
        /// </summary>
        public const int upperTagLevel = 32;

        /// <summary>
        /// The string that represents the opening tag.
        /// </summary>
        public String tagStartOld;

        /// <summary>
        /// The string that represents the closing tag.
        /// </summary>
        public String tagEndOld;

        /// <summary>
        /// The string that represents the replacement opening tag.
        /// </summary>
        public String tagStartNew;

        /// <summary>
        /// The string that represents the replacement closing tag.
        /// </summary>
        public String tagEndNew;

        /// <summary>
        /// The mask for flagging this tag.
        /// </summary>
        private int bitFlag;

        /// <summary>
        /// Method that checks if this tag's flag is set in a particular int.
        /// </summary>
        /// <param name="b">The int in which this tag may be flagged.</param>
        /// <returns>Whether or not this tag is flagged.</returns>
        public bool hasTag(int b)
        {
            if ((b & bitFlag) == bitFlag)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method for (un)setting this tag's bit flag in an int.
        /// </summary>
        /// <param name="bt">The int containing bit flags.</param>
        /// <param name="bl">Whether this tag should be flagged or unflagged.</param>
        /// <returns>The original int with this tag (un)flagged.</returns>
        public int tagFlag(int bt, bool bl)
        {
            if (bl)
                return (bt | bitFlag);
            else
                return (bt & (~bitFlag));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start_old">String that represents the opening tag.</param>
        /// <param name="end_old">String that represents the closing tag.</param>
        /// <param name="start_new">String that represents the replacement opening tag.</param>
        /// <param name="end_new">String that represents the replacement closing tag.</param>
        /// <param name="level">The (unique) precedence level of this tag (<c>0 &lt;= level &lt; upperTagLevel</c>)</param>
        public Tag(String start_old, String end_old, String start_new, String end_new, int level)
        {
            if (level < upperTagLevel)
            {
                tagStartOld = start_old;
                tagEndOld = end_old;
                tagStartNew = start_new;
                tagEndNew = end_new;
                bitFlag = (1 << level);
            }
            else
                throw new Exception(String.Format("Level can only go up to (not including) {0}.", upperTagLevel));
        }
    }

    /// <summary>
    /// Class that provides functionality for nesting (custom) tags in a string correctly.
    /// </summary>
    public class TagNester
    {
        /// <summary>
        /// List of tags (in order of precedence).
        /// </summary>
        private List<Tag> tagList;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TagNester()
        {
            tagList = new List<Tag>();
        }

        /// <summary>
        /// Get the maximum amount of tags that are allowed.
        /// </summary>
        /// <returns>The maximum amount of tags that are allowed.</returns>
        public static int getMaximumTagCount()
        {
            return Tag.upperTagLevel;
        }

        /// <summary>
        /// Method to add a tag. A maximum of Tag.upperTagLevel tags can be added, in total. The order in which
        /// they are added also determines the order of precedence of the tags.
        /// </summary>
        /// <param name="start_old">String that represents the opening tag.</param>
        /// <param name="end_old">String that represents the closing tag.</param>
        /// <param name="start_new">String that represents the replacement opening tag.</param>
        /// <param name="end_new">String that represents the replacement closing tag.</param>
        public void addTag(String start_old, String end_old, String start_new, String end_new)
        {
            int level = tagList.Count;
            if (level < Tag.upperTagLevel)
                tagList.Add(new Tag(start_old, end_old, start_new, end_new, level));
            else
                throw new Exception(String.Format("TagNester.addTag: No more than {0} tags are currently supported.", Tag.upperTagLevel));
        }

        /// <summary>
        /// Method to clear the collection of tags added to this TagNester.
        /// </summary>
        public void clearTags()
        {
            tagList.Clear();
        }

        /// <summary>
        /// Function to fix the nesting of the tags in a string, according to their precedence ordering.
        /// </summary>
        /// <param name="s">The string to be fixed.</param>
        /// <param name="processBareText">An optional function to apply to the actual text in between the tags (not the tags themselves), for instance for character escaping.</param>
        /// <returns>The correctly nested string, with optionally the <c>processBareText</c> function applied to the stripped text.</returns>
        public String fixNesting(String s, Func<String, String> processBareText = null)
        {
            /* This function works by stripping all tags from the string, tallying per character in which tags it is contained, resulting
             * in a clean string. This clean string and the tallying result is then used to build a new string with tags. Tags are added
             * in a hierarchical fashion, using a recursive helper function.
             */

            // Will contain the cleaned string, from which tags are removed.
            StringBuilder cleanString = new StringBuilder();
            // List that keeps a byte with tag bit flags for each character in the cleanString
            List<int> flags = new List<int>();
            // Array to keep track of each tag while walking through the string s, with a bool indicating whether we are inside
            // this tag (true) or not (false).
            bool[] activeTags = new bool[tagList.Count];

            // index of the current character of the cleanString (= current index of the flags list)
            int index_clean = 0;
            // index of the current character in the parameter string
            int index_old = 0;
            while (index_old < s.Length)
            {
                // make sure an entry in the flags list exists for the current clean char
                if (flags.Count != index_clean + 1)
                    flags.Add(0);

                // check if there is a tag at this point, by searching each tag at this point
                bool aTagWasFound = false;
                for (int i = 0; i < tagList.Count && !aTagWasFound; i++)
                {
                    Tag t = tagList[i];
                    // check for either opening or closing string of the current tag
                    // (if found: note that this tag is now (in)active, move forward in the old string, and escape from the loop by noting that a tag was found)
                    if (String.Compare(s, index_old, t.tagStartOld, 0, t.tagStartOld.Length) == 0)
                    {
                        activeTags[i] = true;
                        index_old += t.tagStartOld.Length;
                        aTagWasFound = true;
                    }
                    else if (String.Compare(s, index_old, t.tagEndOld, 0, t.tagEndOld.Length) == 0)
                    {
                        activeTags[i] = false;
                        index_old += t.tagEndOld.Length;
                        aTagWasFound = true;
                    }
                    flags[index_clean] = t.tagFlag(flags[index_clean], activeTags[i]);
                }
                if (!aTagWasFound)
                {
                    // there was no tag here, just a normal character
                    // so add it to the cleanString and move on to the next character
                    cleanString.Append(s[index_old]);
                    index_old++;
                    index_clean++;
                }
            }
            
            // all the active tags per character in the clean string have been tallied, now generate a new string with tags based on this
            return stitchString(cleanString.ToString(), 0, cleanString.Length, flags, 0, processBareText);
        }

        /// <summary>
        /// Recursive function that generates a string containing tags for (a subsequence of) an input string without tags, but with
        /// information for each character <c>c</c> in the string of the tags that are active for <c>c</c>.
        /// The subsequence is defined as all characters of string <value>s</value> with zero-based index <c>i</c> for which <c><value>index_start</value> &lt;= i &lt; <value>index_end</value></c>.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="index_start">Zero-based start index of the subsequence of the input string to be processed.</param>
        /// <param name="index_end">Zero-based index of the last character up to (and not including) which the subsequence runs.</param>
        /// <param name="flags_per_char">List of flags per character, where the index in the list corresponds to the index of the character in the string <value>s</value>.</param>
        /// <param name="tagIndex">The highest level tag to include in the processing.</param>
        /// <param name="processBareText">Optional function to run on the bare text (i.e. the text without tags)</param>
        /// <returns>String with correctly nested tags.</returns>
        private String stitchString(String s, int index_start, int index_end, List<int> flags_per_char, int tagIndex, Func<String, String> processBareText = null)
        {
            // Treat s[i] for (index_start <= i < index_end)
            if (index_start == index_end)
                // empty domain
                return String.Empty;
            else
            {
                // another stopping condition is if we have reached a tag index which doesn't exist
                if (tagList.Count <= tagIndex)
                {
                    // this means there is no more tag to check for, so we just return the bare subsequence
                    // (optionally with a function applied to it)
                    String result = s.Substring(index_start, index_end - index_start);
                    if (processBareText == null)
                        return result;
                    else
                        return processBareText(result);
                }
                else
                {
                    // no stopping condition was encountered

                    // for the entire subsequence, identify blocks for which the current tag (tagIndex) applies/doesn't apply,
                    // circumfix tags in the former case, call self recursively on that block with the next tag (tagIndex+1),
                    // and concatenate the result from all these blocks and return it.
                    StringBuilder concatenation = new StringBuilder();
                    bool prevVal = false; // (whether or not the tag is active)
                    int blockStart = index_start;

                    for (int i = index_start; i < index_end; i++)
                    {
                        // check if prevVal equals the tag's activeness in for s[i], if not
                        if (prevVal != tagList[tagIndex].hasTag(flags_per_char[i]))
                        {
                            // a block has ended, range is [blockStart .. i)
                            String sub1 = stitchString(s, blockStart, i, flags_per_char, tagIndex + 1, processBareText);
                            if (prevVal)
                                // tag was active, so should be circumfixed
                                concatenation.Append(tagList[tagIndex].tagStartNew).Append(sub1).Append(tagList[tagIndex].tagEndNew);
                            else
                                // tag was not active
                                concatenation.Append(sub1);
                            // start new block
                            blockStart = i;
                            prevVal = !prevVal;
                        }
                    }
                    // end of subsequence was reached, so another block has ended, range is [blockStart .. index_end)
                    String sub2 = stitchString(s, blockStart, index_end, flags_per_char, tagIndex + 1, processBareText);
                    if (prevVal)
                        // tag was active, so should be circumfixed
                        concatenation.Append(tagList[tagIndex].tagStartNew).Append(sub2).Append(tagList[tagIndex].tagEndNew);
                    else
                        // tag was not active
                        concatenation.Append(sub2);
                    
                    return concatenation.ToString();
                }
            }
        }
    }
}
