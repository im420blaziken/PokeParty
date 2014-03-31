/**
 * This file is part of PokeParty.
 * 
 * Copyright (C) 2014 Ashlee Katzenbaer
 * All Rights Reserved.
 * 
 * @github im420blaziken
 *  
 * PokeParty is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * PokeParty is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PokeParty.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace PokeParty
{
    class PokeSave
    {
        private MemoryStream _mem;
        private List<Pokemon> _team;
        
        enum OffsetType {
            SAVESTATE,
            TEAM_LIST,
            BADGES
        }
        private Dictionary<GameType, Dictionary<OffsetType, uint>> _offsets = new Dictionary<GameType, Dictionary<OffsetType, uint>>() {
            {
                GameType.RBY, 
                new Dictionary<OffsetType, uint>() { 
                    { OffsetType.SAVESTATE, 0x125B2 },
                    { OffsetType.TEAM_LIST, 0x2F2C },
                }
            }, 
            {
                GameType.CRYSTAL, 
                new Dictionary<OffsetType, uint>() {
                    { OffsetType.SAVESTATE, 0x137EE },
                    { OffsetType.TEAM_LIST, 0x2865 },
                }
            },
            {
                GameType.EMERALD, 
                new Dictionary<OffsetType, uint>() {
                    { OffsetType.SAVESTATE, 0x2B894 },
                    { OffsetType.TEAM_LIST, 0x1234 },
                    //{ OffsetType.BADGES, 0x3A7D }
                    { OffsetType.BADGES, 0x396C }
                }
            },
        };

        GameType _currentGame = GameType.EMERALD;
        public GameType CurrentGame
        {
            get { return _currentGame; }
        }

        public PokeSave(string path)
        {
            if (!File.Exists(path)) throw new ArgumentException("Save file does not exist: '" + path + "'");

            try
            {
                _mem = new MemoryStream();
                using (FileStream fileStream = File.OpenRead(path))
                {
                    using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        decompressedStream.CopyTo(_mem);
                    }
                }
                _mem.Position = 0;

                DetectVersion();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Unable to load save: " + e.Message);
            }
        }

        enum FormatStructure
        {
            GROWTH = 0,
            MOVES,
            EVS,
            MISC
        };
        static FormatStructure[,] FormatOrder = new FormatStructure[24, 4] 
        {
            {FormatStructure.GROWTH, FormatStructure.MOVES, FormatStructure.EVS, FormatStructure.MISC},
            {FormatStructure.GROWTH, FormatStructure.MOVES, FormatStructure.MISC, FormatStructure.EVS},
            {FormatStructure.GROWTH, FormatStructure.EVS, FormatStructure.MOVES, FormatStructure.MISC},
            {FormatStructure.GROWTH, FormatStructure.EVS, FormatStructure.MISC, FormatStructure.MOVES},
            {FormatStructure.GROWTH, FormatStructure.MISC, FormatStructure.MOVES, FormatStructure.EVS},
            {FormatStructure.GROWTH, FormatStructure.MISC, FormatStructure.EVS, FormatStructure.MOVES},
            {FormatStructure.MOVES, FormatStructure.GROWTH, FormatStructure.EVS, FormatStructure.MISC},
            {FormatStructure.MOVES, FormatStructure.GROWTH, FormatStructure.MISC, FormatStructure.EVS},
            {FormatStructure.MOVES, FormatStructure.EVS, FormatStructure.GROWTH, FormatStructure.MISC},
            {FormatStructure.MOVES, FormatStructure.EVS, FormatStructure.MISC, FormatStructure.GROWTH},
            {FormatStructure.MOVES, FormatStructure.MISC, FormatStructure.GROWTH, FormatStructure.EVS},
            {FormatStructure.MOVES, FormatStructure.MISC, FormatStructure.EVS, FormatStructure.GROWTH},
            {FormatStructure.EVS, FormatStructure.GROWTH, FormatStructure.MOVES, FormatStructure.MISC},
            {FormatStructure.EVS, FormatStructure.GROWTH, FormatStructure.MOVES, FormatStructure.EVS},
            {FormatStructure.EVS, FormatStructure.MOVES, FormatStructure.GROWTH, FormatStructure.MISC},
            {FormatStructure.EVS, FormatStructure.MOVES, FormatStructure.MISC, FormatStructure.GROWTH},
            {FormatStructure.EVS, FormatStructure.MISC, FormatStructure.GROWTH, FormatStructure.MOVES},
            {FormatStructure.EVS, FormatStructure.MISC, FormatStructure.MOVES, FormatStructure.GROWTH},
            {FormatStructure.MISC, FormatStructure.GROWTH, FormatStructure.MOVES, FormatStructure.EVS},
            {FormatStructure.MISC, FormatStructure.GROWTH, FormatStructure.EVS, FormatStructure.MOVES},
            {FormatStructure.MISC, FormatStructure.MOVES, FormatStructure.GROWTH, FormatStructure.EVS},
            {FormatStructure.MISC, FormatStructure.MOVES, FormatStructure.EVS, FormatStructure.GROWTH},
            {FormatStructure.MISC, FormatStructure.EVS, FormatStructure.GROWTH, FormatStructure.MOVES},
            {FormatStructure.MISC, FormatStructure.EVS, FormatStructure.MOVES, FormatStructure.GROWTH}
        };

        private static Dictionary<string, GameType> KnownVersions = new Dictionary<string,GameType>() { 
            {"POKEMON YELLOW", GameType.RBY}, // TODO Add Red and Blue and double-check the savestate offset.
            {"POKEMON EMER", GameType.EMERALD},
            {"PM_CRYSTAL", GameType.CRYSTAL},
        };
        private void DetectVersion()
        {
            if (_mem != null)
            {
                BinaryReader br = new BinaryReader(_mem, System.Text.Encoding.ASCII);
                br.BaseStream.Position = 0x4;
                string rom_name = new string(br.ReadChars(16));
                foreach (KeyValuePair<string, GameType> knownVersion in KnownVersions)
                {
                    if (rom_name.StartsWith(knownVersion.Key))
                    {
                        _currentGame = knownVersion.Value;
                        Console.WriteLine("PokeSave detected version: " + knownVersion.Key);
                        return;
                    }
                }
            }
        }

        private static Dictionary<GameType, byte[]> asciiTable = new Dictionary<GameType, byte[]>()
        {
            {
                GameType.EMERALD, 
                new byte[256] {
                    0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x21, 0x3F, 0x2E, 0x2D, 0x00,
                    0x00, 0x22, 0x22, 0x27, 0x27, 0x20, 0x20, 0x00, 0x2C, 0x00, 0x2F, 0x41, 0x42, 0x43, 0x44, 0x45,
                    0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55,
                    0x56, 0x57, 0x58, 0x59, 0x5A, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B,
                    0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                }
            },
        };
        public List<Pokemon> Team
        {
            get
            {
                if (_mem == null) return null;

                if (_team == null) // Fetch team data
                {
                    _team = new List<Pokemon>();
                    if (_currentGame == GameType.RBY || _currentGame == GameType.CRYSTAL)
                    {
                        long _pkmn_team_position = _offsets[_currentGame][OffsetType.SAVESTATE] + _offsets[_currentGame][OffsetType.TEAM_LIST];
                        Console.WriteLine("Team position is: {0:X}", _pkmn_team_position);
                        _mem.Position = _pkmn_team_position;
                        int count = _mem.ReadByte();
                        for (int i = 0; i < count; i++)
                        {
                            if (_currentGame == GameType.RBY)
                            {
                                byte species, level;
                                _mem.Position = _pkmn_team_position + 8 + i * 0x2C;
                                species = (byte)_mem.ReadByte();
                                _mem.Seek(0x2, SeekOrigin.Current);
                                level = (byte)_mem.ReadByte();
                                _team.Add(new Pokemon(_currentGame, (short)species, level));
                            }
                            else if (_currentGame == GameType.CRYSTAL)
                            {
                                // Check to make sure it's not an egg
                                _mem.Position = _pkmn_team_position + 1 + i;
                                if (_mem.ReadByte() == 0xFD) // is egg
                                    _team.Add(new Pokemon(_currentGame, 0xFD, 0));
                                else
                                {
                                    // Read Full Pokemon Data Structure, not just species identifier
                                    byte species, level;
                                    _mem.Position = _pkmn_team_position + 8 + i * 0x30;
                                    species = (byte)_mem.ReadByte();
                                    _mem.Seek(0x1E, SeekOrigin.Current);
                                    level = (byte)_mem.ReadByte();
                                    _team.Add(new Pokemon(_currentGame, (short)species, level));
                                }
                            }
                        }
                        //if (_data.ReadByte() != 0xFF) throw new InvalidDataException("Unable to locate expected species list terminator byte at 0x" + String.Format("{0:X}", (_data.Position - 1)));
                    }
                    else if (_currentGame == GameType.EMERALD)
                    {
                        BinaryReader br = new BinaryReader(_mem, System.Text.Encoding.ASCII);
                        long _pkmn_team_position = _offsets[_currentGame][OffsetType.SAVESTATE] + _offsets[_currentGame][OffsetType.TEAM_LIST];
                        br.BaseStream.Position = _pkmn_team_position;

                        int count = br.ReadByte();      // bulbapedia is wrong. this is a byte. really? a dword for a value that is max 6?
                        br.ReadInt16();                 // not sure if this is right, but it works.

                        _pkmn_team_position = br.BaseStream.Position;
                        for (int i = 0; i < count; i++)
                        {
                            br.BaseStream.Position = _pkmn_team_position + 100 * i;
                            int personality = br.ReadInt32();   // Personality
                            int otid = br.ReadInt32();          // OT ID
                            byte[] _nickname = br.ReadBytes(10);        // Nickname
                            Console.Write("Found raw nickname: ");
                            for (int m = 0; m < 10; m++)
                            {
                                Console.Write("{0:X} ", _nickname[m]);
                                _nickname[m] = asciiTable[GameType.EMERALD][_nickname[m]];
                            }
                            Console.WriteLine();
                            //string nickname = new string(_nickname, 0, 10);
                            string nickname = System.Text.Encoding.ASCII.GetString(_nickname, 0, 10);
                            Console.WriteLine("Found nickname: ", nickname);
                            br.BaseStream.Seek(2, SeekOrigin.Current); // Language
                            br.BaseStream.Seek(7, SeekOrigin.Current); // OT Name
                            br.BaseStream.Seek(1, SeekOrigin.Current); // Mark
                            br.BaseStream.Seek(2, SeekOrigin.Current); // Checksum
                            br.BaseStream.Seek(2, SeekOrigin.Current); // ????

                            uint order = (uint)personality % 24;

                            int encryption = otid ^ personality;
                            byte[] data = new byte[48];
                            for (int j = 0; j < 48; j += 4)
                            {
                                int _bytes = br.ReadInt32() ^ encryption;
                                data[j] = (byte)((_bytes >> 24) & 0xFF);
                                data[j + 1] = (byte)((_bytes >> 16) & 0xFF);
                                data[j + 2] = (byte)((_bytes >> 8) & 0xFF);
                                data[j + 3] = (byte)(_bytes & 0xFF);
                            }

                            BinaryWriter dataWriter = new BinaryWriter(new MemoryStream());
                            dataWriter.Write(data);

                            BinaryReader dataReader = new BinaryReader(dataWriter.BaseStream);
                            dataReader.BaseStream.Position = 0;

                            short species = -1;
                            int iv = 0;
                            for (int j = 0; j < 4; j++)
                            {
                                switch (FormatOrder[order, j])
                                {
                                    case FormatStructure.GROWTH:
                                        dataReader.ReadInt16(); // Item Held
                                        species = IPAddress.HostToNetworkOrder(dataReader.ReadInt16()); // Species
                                        dataReader.BaseStream.Seek(8, SeekOrigin.Current);
                                        break;
                                    case FormatStructure.MISC:
                                        dataReader.ReadInt32();
                                        iv = IPAddress.HostToNetworkOrder(dataReader.ReadInt32());
                                        dataReader.ReadInt32();
                                        break;
                                    default:
                                        dataReader.BaseStream.Seek(12, SeekOrigin.Current);
                                        break;
                                }
                            }
                            if ((iv & 1 << 30) > 0) species = 0x19C;
                            Console.WriteLine("IV: " + iv);

                            Pokemon.StatusType status_condition = (Pokemon.StatusType)br.ReadInt32(); // Status Condition
                            byte level = br.ReadByte(); // Level
                            br.ReadByte(); // Pokerus remaining
                            short current_hp = br.ReadInt16();
                            short total_hp = br.ReadInt16();

                            Pokemon p = new Pokemon(_currentGame, species, level);
                            p.Nickname = nickname;
                            p.StatusCondition = status_condition;
                            p.CurrentHP = current_hp;
                            p.TotalHP = total_hp;
                            Console.WriteLine("Pkmn " + i + " is species: " + p.Species + " (" + p.Name + "), level: " + p.Level + ", current hp: " + p.CurrentHP + ", total hp: " + p.TotalHP);
                            _team.Add(p);
                        }
                    }
                }
                return _team;
            }
        }

        byte _badges = 0x00;
        public byte Badges
        {
            get
            {
                if (_mem == null) return 0;

                if (_badges == 0) // Fetch badge data
                {
                    if (_currentGame == GameType.EMERALD)
                    {
                        BinaryReader br = new BinaryReader(_mem, System.Text.Encoding.UTF32);
                        long _pkmn_team_position = _offsets[_currentGame][OffsetType.SAVESTATE] + _offsets[_currentGame][OffsetType.BADGES];
                        br.BaseStream.Position = _pkmn_team_position;

                        byte[] buf = new byte[512];
                        br.Read(buf, 0, 512);
                        for (int i = 0; i < 500; i++)
                        {
                            // 87 FD 6D 73 EB FD 6C FD 73
                            //if (buf[i] == 0x87 && buf[i+1] == 0xFD && buf[i+3] == 0x73 && buf[i+4] == 0xEB && buf[i+5] == 0xFD && buf[i+6] == 0x6C)
                            if (buf[i] == 0x80 && buf[i + 1] == 0xFF && buf[i + 2] == 0xDD && buf[i + 5] == 0xBD)
                            {
                                //_badges = buf[i + 0x38];
                                _badges = buf[i + 0x45];
                                /*int zeroes = 0;
                                while (zeroes < 3)
                                {
                                    if (br.ReadByte() == 0) zeroes++;
                                    else zeroes = 0;
                                }*/
                                break;
                            }
                        }
                        return _badges; // gg, ez.
                    }
                }
                return 0;
            }
        }
    }
}
