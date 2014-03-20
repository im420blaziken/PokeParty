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
                    { OffsetType.BADGES, 0x3A56 }
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
                        BinaryReader br = new BinaryReader(_mem, System.Text.Encoding.UTF32);
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
                            br.BaseStream.Seek(10, SeekOrigin.Current); // Nickname
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
                            for (int j = 0; j < 4; j++)
                            {
                                switch (FormatOrder[order, j])
                                {
                                    case FormatStructure.GROWTH:
                                        dataReader.ReadInt16(); // Item Held
                                        species = IPAddress.HostToNetworkOrder(dataReader.ReadInt16()); // Species
                                        dataReader.BaseStream.Seek(8, SeekOrigin.Current);
                                        break;
                                    default:
                                        dataReader.BaseStream.Seek(12, SeekOrigin.Current);
                                        break;
                                }
                            }

                            br.ReadInt32(); // Status ailment
                            byte level = br.ReadByte(); // Level

                            Pokemon p = new Pokemon(_currentGame, species, level);
                            Console.WriteLine("Pkmn " + i + " is species: " + p.Species + " (" + p.Name + "), level: " + p.Level);
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
                            if (buf[i] == 0x87 && buf[i+1] == 0xFD && buf[i+2] == 0x6D && buf[i+3] == 0x73 && buf[i+4] == 0xEB && buf[i+5] == 0xFD && buf[i+6] == 0x6C
                                && buf[i + 7] == 0xFD && buf[i + 8] == 0x73)
                            {
                                _badges = buf[i + 0x38];
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
