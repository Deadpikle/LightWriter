using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace LightWriter.App_Code
{
    /// <summary>
    /// BlockListSaveLoader
    /// Last modified on 11/26/2013
    /// Allows for saving and loading of block list
    /// algorithms. Also manages deletion of old (overwritten)
    /// algorithms.
    /// In the future, it might be nice to set up a return system
    /// where all functions return a <string, object> dictionary
    /// so that errors are easier to get to the user.
    /// </summary>
    public class BlockListSaverLoader
    {
        // In order to save a new type of block, update the deleteAlgorithm function
        // and the SaveUserAlgorithm foreach loop. (roughly 10 lines of code per block type)

        /// <summary>
        /// Deletes an algorithm from the table by deleting all of its blocks followed
        /// by the algorithm itself.
        /// </summary>
        /// <param name="alg">The algorithm to delete</param>
        /// <param name="dataContext">The db data context to delete from</param>
        private static void deleteAlgorithm(Algorithm alg, LightWriterDataContext dataContext)
        {
            // Go through each block table and remove the blocks with this alg id
            var algID = alg.ID;
            // Single block
            dataContext.SingleBlocks.DeleteAllOnSubmit(alg.SingleBlocks);
            // Comparison block
            dataContext.ComparisonBlocks.DeleteAllOnSubmit(alg.ComparisonBlocks);
            // Range block
            dataContext.RangeBlocks.DeleteAllOnSubmit(alg.RangeBlocks);
            // Circle block
            dataContext.CircleBlocks.DeleteAllOnSubmit(alg.CircleBlocks);
            // etc...
            dataContext.SingleMoveBlocks.DeleteAllOnSubmit(alg.SingleMoveBlocks);
            dataContext.ComparisonMoveBlocks.DeleteAllOnSubmit(alg.ComparisonMoveBlocks);
            dataContext.RangeMoveBlocks.DeleteAllOnSubmit(alg.RangeMoveBlocks);
            dataContext.CommentBlocks.DeleteAllOnSubmit(alg.CommentBlocks);
            // delete the rules
            dataContext.Rules.DeleteAllOnSubmit(alg.Rules);
            dataContext.SubmitChanges(); // submit deletions of saved blocks and rules
            // Now delete the algorithm
            dataContext.Algorithms.DeleteOnSubmit(alg);
            dataContext.SubmitChanges();
        }

        /// <summary>
        /// Deletes an algorithm by a user-chosen name (this function is sent the ID of the alg
        /// with that name). Called "ByName" since the user selects the alg by name, not id.
        /// </summary>
        /// <param name="id">ID of algorithm to delete</param>
        /// <returns>"Success" on success, or a failure message if not.</returns>
        public static string DeleteAlgorithmByName(int id)
        {
            LightWriterDataContext dataContext = new LightWriterDataContext();
            Algorithm alg = dataContext.Algorithms.Where(c => c.ID == id).FirstOrDefault();
            if (alg != null)
            {
                deleteAlgorithm(alg, dataContext);
                return "Success";
            }
            else return "Failure: Algorithm does not exist with ID: " + id; // shouldn't happen since we loaded 'em a second ago, but safety precaution
        }

        /// <summary>
        /// Saves a user algorithm, one block at a time.
        /// First checks to see if an algorithm already exists for this user with name algorithmName.
        /// If one does, it deletes that algorithm, since we don't want 2 "duplicate" algorithms
        /// for a user.
        /// Then, it creates a new algorithm obj, saves it to the db, then parses the blockList json
        /// and saves each block by its type.
        /// </summary>
        /// <param name="userID">The user ID of the person we are saving this algorithm for.</param>
        /// <param name="algorithmName">The name of the algorithm to save for the user.</param>
        /// <param name="blockListJson">The JSON representation of the block list.</param>
        /// <param name="rulesJson">The JSON representation of the algorithm's rules.</param>
        /// <returns>Success message in dict if the process succeeded, and some other error string otherwise [in a dict with "Error" key].</returns>
        public static Dictionary<string, object> SaveUserAlgorithm(int userID, string algorithmName, string blockListJson, string rulesJson)
        {
            LightWriterDataContext dataContext = new LightWriterDataContext();
            // First see if algorithmName already exists in the db
            Algorithm alg = dataContext.Algorithms.Where(c => c.UserID == userID && c.Name == algorithmName).FirstOrDefault();
            if (alg != null)
            { 
                // need to erase the current algorithm and its associated blocks before creating a new one
                // This removes the need to "update" blocks [a big pain] and instead we just save a whole new algorithm
                deleteAlgorithm(alg, dataContext);
            }
            // Now save a new algorithm slot into the table
            Algorithm latestUserAlgorithm = new Algorithm();
            latestUserAlgorithm.UserID = userID;
            latestUserAlgorithm.Name = algorithmName;
            latestUserAlgorithm.DateCreated = DateTime.Now;
            dataContext.Algorithms.InsertOnSubmit(latestUserAlgorithm);
            dataContext.SubmitChanges(); // save this algorithm into the table
            try // In case something bad happens, we can delete the algorithm that failed to save
            { 
                // We now have the algorithm ID of the algorithm being saved!
                var latestAlgorithmID = latestUserAlgorithm.ID;
                // Alright. Now insert the blocks into the database!
                // First, parse the Json
                var userBlockList = Json.Decode(blockListJson);
                var blockIDs = userBlockList.GetDynamicMemberNames();
                foreach (string blockID in blockIDs) // loop through blocks and save 'em
                {
                    var block = userBlockList[blockID];
                    string type = block.type;
                    int position = block.position; // position in the list
                    if (type.Equals("SingleBlock"))
                    {
                        // insert single block into db
                        SingleBlock sb = new SingleBlock();
                        sb.AlgorithmID = latestAlgorithmID;
                        sb.RowID = block.rowId;
                        sb.ColumnID = block.columnId;
                        sb.ActionString = block.actionString;
                        sb.SetToColor = block.setToColor;
                        sb.Position = position;
                        dataContext.SingleBlocks.InsertOnSubmit(sb);
                    }
                    else if (type.Equals("Comparison"))
                    {
                        ComparisonBlock cb = new ComparisonBlock();
                        cb.AlgorithmID = latestAlgorithmID;
                        cb.Position = position;
                        cb.RowOrColumnID = block.rowOrColumnId;
                        cb.ComparisonActionString = block.comparisonActionString;
                        cb.ActionString = block.actionString;
                        cb.SetToColor = block.setToColor;
                        dataContext.ComparisonBlocks.InsertOnSubmit(cb);
                    }
                    else if (type.Equals("Range"))
                    {
                        RangeBlock rb = new RangeBlock();
                        rb.AlgorithmID = latestAlgorithmID;
                        rb.Position = position;
                        rb.ActionString = block.actionString;
                        rb.SetToColor = block.setToColor;
                        // YES I REALIZE THIS IS MESSY. YES IT IS UGLY. NO I DON'T HAVE TIME TO FIX IT.
                        // Don't know why the DB accepts ints -> varchar for the comparison block but not this block :\
                        try
                        {
                            int leftRowOrColumnID = (int)block.leftRowOrColumnId; // see if an int leftRowOrColumnId was sent
                            rb.LeftRowOrColumnID = leftRowOrColumnID.ToString(); // db wants a varchar
                        }
                        catch (Exception e)
                        {
                            rb.LeftRowOrColumnID = block.leftRowOrColumnId; // it was actually a string
                        }
                        try
                        {
                            int rightRowOrColumnId = (int)block.rightRowOrColumnId; // see if an int rightRowOrColumnId was sent
                            rb.RightRowOrColumnID = rightRowOrColumnId.ToString(); // db wants a varchar
                        }
                        catch (Exception e)
                        {
                            rb.RightRowOrColumnID = block.rightRowOrColumnId; // it was actually a string
                        }
                        dataContext.RangeBlocks.InsertOnSubmit(rb);
                    }
                    else if (type.Equals("CircleBlock"))
                    {
                        CircleBlock circleBlock = new CircleBlock();
                        circleBlock.AlgorithmID = latestAlgorithmID;
                        circleBlock.Position = position;
                        circleBlock.SetToColor = block.setToColor;
                        circleBlock.ActionString = block.actionString;
                        circleBlock.RowID = block.rowId;
                        circleBlock.ColumnID = block.columnId;
                        try
                        {
                            var radius = Int32.Parse(block.radius); // see if a string radius was sent
                            circleBlock.Radius = radius;
                        }
                        catch (Exception e)
                        {
                            circleBlock.Radius = block.radius; // it was actually an int
                        }
                        dataContext.CircleBlocks.InsertOnSubmit(circleBlock);
                    }
                    else if (type.Equals("SingleMoveBlock"))
                    {
                        SingleMoveBlock smb = new SingleMoveBlock();
                        smb.AlgorithmID = latestAlgorithmID;
                        smb.RowID = block.rowId;
                        smb.ColumnID = block.columnId;
                        smb.ActionString = block.actionString;
                        smb.SetToColor = block.setToColor;
                        smb.Position = position;
                        smb.Direction = block.direction;
                        try
                        {
                            var numberBlocksToMove = Int32.Parse(block.numberBlocksToMove); // see if a string radius was sent
                            smb.NumberBlocksToMove = numberBlocksToMove;
                        }
                        catch (Exception e)
                        {
                            smb.NumberBlocksToMove = block.numberBlocksToMove; // it was actually an int
                        }
                        try
                        {
                            var numberTicksBeforeChange = Int32.Parse(block.numberTicksBeforeChange); // see if a string radius was sent
                            smb.NumberTicksBeforeChange = numberTicksBeforeChange;
                        }
                        catch (Exception e)
                        {
                            smb.NumberTicksBeforeChange = block.numberTicksBeforeChange; // it was actually an int
                        }
                        dataContext.SingleMoveBlocks.InsertOnSubmit(smb);
                    }
                    else if (type.Equals("ComparisonMoveBlock"))
                    {
                        ComparisonMoveBlock cmb = new ComparisonMoveBlock();
                        cmb.AlgorithmID = latestAlgorithmID;
                        cmb.RowOrColumnID = block.rowOrColumnId;
                        cmb.ActionString = block.actionString;
                        cmb.ComparisonActionString = block.comparisonActionString;
                        cmb.SetToColor = block.setToColor;
                        cmb.Position = position;
                        cmb.Direction = block.direction;
                        try
                        {
                            var numberBlocksToMove = Int32.Parse(block.numberBlocksToMove); // see if a string radius was sent
                            cmb.NumberBlocksToMove = numberBlocksToMove;
                        }
                        catch (Exception e)
                        {
                            cmb.NumberBlocksToMove = block.numberBlocksToMove; // it was actually an int
                        }
                        try
                        {
                            var numberTicksBeforeChange = Int32.Parse(block.numberTicksBeforeChange); // see if a string radius was sent
                            cmb.NumberTicksBeforeChange = numberTicksBeforeChange;
                        }
                        catch (Exception e)
                        {
                            cmb.NumberTicksBeforeChange = block.numberTicksBeforeChange; // it was actually an int
                        }
                        dataContext.ComparisonMoveBlocks.InsertOnSubmit(cmb);
                    }
                    else if (type.Equals("RangeMove"))
                    {
                        RangeMoveBlock rmb = new RangeMoveBlock();
                        rmb.AlgorithmID = latestAlgorithmID;
                        rmb.Position = position;
                        rmb.ActionString = block.actionString;
                        rmb.SetToColor = block.setToColor;
                        rmb.Direction = block.direction;
                        // YES I REALIZE THIS IS MESSY. YES IT IS UGLY. NO I DON'T HAVE TIME TO FIX IT.
                        // Don't know why the DB accepts ints -> varchar for the comparison block but not this block :\
                        try
                        {
                            int leftRowOrColumnID = (int)block.leftRowOrColumnId; // see if an int leftRowOrColumnId was sent
                            rmb.LeftRowOrColumnID = leftRowOrColumnID.ToString(); // db wants a varchar
                        }
                        catch (Exception e)
                        {
                            rmb.LeftRowOrColumnID = block.leftRowOrColumnId; // it was actually a string
                        }
                        try
                        {
                            int rightRowOrColumnId = (int)block.rightRowOrColumnId; // see if an int rightRowOrColumnId was sent
                            rmb.RightRowOrColumnID = rightRowOrColumnId.ToString(); // db wants a varchar
                        }
                        catch (Exception e)
                        {
                            rmb.RightRowOrColumnID = block.rightRowOrColumnId; // it was actually a string
                        }

                        try
                        {
                            var numberBlocksToMove = Int32.Parse(block.numberBlocksToMove); // see if a string numberBlocksToMove was sent
                            rmb.NumberBlocksToMove = numberBlocksToMove;
                        }
                        catch (Exception e)
                        {
                            rmb.NumberBlocksToMove = block.numberBlocksToMove; // it was actually an int
                        }
                        try
                        {
                            var numberTicksBeforeChange = Int32.Parse(block.numberTicksBeforeChange); // see if a string numberTicksBeforeChange was sent
                            rmb.NumberTicksBeforeChange = numberTicksBeforeChange;
                        }
                        catch (Exception e)
                        {
                            rmb.NumberTicksBeforeChange = block.numberTicksBeforeChange; // it was actually an int
                        }
                        dataContext.RangeMoveBlocks.InsertOnSubmit(rmb);
                    }
                    else if (type.Equals("Comment"))
                    {
                        CommentBlock cb = new CommentBlock();
                        cb.AlgorithmID = latestAlgorithmID;
                        cb.Position = position;
                        cb.Comment = block.comment;
                        dataContext.CommentBlocks.InsertOnSubmit(cb);
                    }
                }
                // Now save the rules!
                Rule algRules = new Rule();
                var algRulesDecoded = Json.Decode(rulesJson);
                algRules.AlgorithmID = latestAlgorithmID;
                algRules.DefaultBoxColor = algRulesDecoded["DEFAULT_BOX_COLOR"];
                try
                {
                    algRules.MSPerTick = Int32.Parse(algRulesDecoded["MS_PER_TICK"]); // sent as string
                }
                catch (Exception e)
                {
                    algRules.MSPerTick = algRulesDecoded["MS_PER_TICK"]; // sent as int
                }
                bool colorMixing = algRulesDecoded["COLOR_MIXING"];
                algRules.ColorMixing = (byte)(colorMixing == true ? 1 : 0);
                dataContext.Rules.InsertOnSubmit(algRules);
                // Now save all the things!
                dataContext.SubmitChanges(); // Save all the blocks!
                if (latestUserAlgorithm != null)
                {
                    Dictionary<string, object> successDict = new Dictionary<string, object>() {
                        { "AlgID", latestUserAlgorithm.ID }
                    };
                    return successDict;
                }
                else
                {
                    Dictionary<string, object> errorDictionary = new Dictionary<string, object>() {
                        { "Error", "Something went wrong trying to save your algorithm. The algorithm it was saved under is null!" }
                    };
                    return errorDictionary;
                }
            }
            catch (Exception e)
            {
                if (latestUserAlgorithm != null)
                    deleteAlgorithm(latestUserAlgorithm, dataContext);
                Dictionary<string, object> errorDictionary = new Dictionary<string, object>() {
                    { "Error", "Something went wrong trying to save your algorithm. Server error: " + e.Message }
                };
                return errorDictionary;
            }
        }

        /// <summary>
        /// Simply a public function that can be called to load the algorithm names saved by user with
        /// ID of userID;
        /// </summary>
        /// <param name="userID">The userID of the user to load algorithm names for</param>
        /// <returns>A string array of algorithm names</returns>
        public static Array LoadUserAlgorithmNames(int userID)
        {
            LightWriterDataContext dataContext = new LightWriterDataContext();
            return dataContext.Algorithms.Where(c => c.UserID == userID).Select(c => c.ID + ";;;;" + c.Name).ToArray();
        }

        /// <summary>
        /// Loads the user algorithm saved in the database by loading the
        /// algorithm first and then the algorithm's associated blocks.
        /// </summary>
        /// <param name="algorithmID">ID of the algorithm to load</param>
        /// <returns>A dictionary of string->object pairs that contains
        /// details about the algorithm as well as the blocks saved
        /// for the algorithm.</returns>
        public static Dictionary<string, object> LoadUserAlgorithm(int algorithmID)
        {
            Dictionary<string, object> algorithm = new Dictionary<string, object>();
            LightWriterDataContext dataContext = new LightWriterDataContext();
            Algorithm alg = dataContext.Algorithms.Where(c => c.ID == algorithmID).FirstOrDefault();
            if (alg != null)
            {
                // Load in the algorithm details
                algorithm["id"] = alg.ID;
                algorithm["name"] = alg.Name;
                algorithm["dateCreated"] = alg.DateCreated;
                // Now load in all the blocks!
                // Single block
                algorithm["singleBlocks"] = alg.SingleBlocks.ToArray();
                // Comparison
                algorithm["comparisonBlocks"] = alg.ComparisonBlocks.ToArray();
                // Comparison Range
                algorithm["rangeBlocks"] = alg.RangeBlocks.ToArray();
                // Circle
                algorithm["circleBlocks"] = alg.CircleBlocks.ToArray();
                // etc...
                algorithm["singleMoveBlocks"] = alg.SingleMoveBlocks.ToArray();
                algorithm["comparisonMoveBlocks"] = alg.ComparisonMoveBlocks.ToArray();
                algorithm["rangeMoveBlocks"] = alg.RangeMoveBlocks.ToArray();
                algorithm["commentBlocks"] = alg.CommentBlocks.ToArray();
                // Make sure to return the rules too!
                algorithm["rules"] = alg.Rules.ToArray();
            return algorithm;
            }
            else
            {
                Dictionary<string, object> errorDictionary = new Dictionary<string, object>() {
                    { "Error", "Algorithm ID is invalid."}
                };
                return errorDictionary;
            }
        }
    }
}