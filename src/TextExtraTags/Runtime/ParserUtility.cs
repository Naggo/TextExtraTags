using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TextExtraTags {
    public static class ParserUtility {
        public static Parser GetParser(string name) {
            return TextExtraTagsSettings.Instance.GetParser(name);
        }

        public static bool TryParseTag(
            ReadOnlySpan<char> source, int startIndex, out ParserTagData result
        ) {
            int lbIndex = -1;
            int sepIndex = -1;

            for (int i = startIndex; i < source.Length; i++) {
                char c = source[i];

                if (c == '<') {
                    lbIndex = i;
                } else if (c == '=' && sepIndex < lbIndex) {
                    sepIndex = i;
                } else if (c == '>' && 0 <= lbIndex){
                    result = new ParserTagData(source, lbIndex, i, sepIndex);
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static bool IsRichTextTag(in ParserTagData tagData) {
            ReadOnlySpan<char> name = tagData.Name;
            if (name.Length == 0) return false;

            switch (name[0]) {
                case 'a':
                    if (name.SequenceEqual("align")) {
                        return true;
                    } else if (name.SequenceEqual("allcaps")) {
                        return true;
                    } else if (name.SequenceEqual("alpha")) {
                        return true;
                    }
                    break;
                case 'b':
                    if (name.SequenceEqual("b")) {
                        return true;
                    } else if (name.SequenceEqual("br")) {
                        return true;
                    }
                    break;
                case 'c':
                    if (name.SequenceEqual("color")) {
                        return true;
                    } else if (name.SequenceEqual("cspace")) {
                        return true;
                    }
                    break;
                case 'f':
                    if (name.SequenceEqual("font")) {
                        return true;
                    } else if (name.SequenceEqual("font-weight")) {
                        return true;
                    }
                    break;
                case 'g':
                    if (name.SequenceEqual("gradient")) {
                        return true;
                    }
                    break;
                case 'i':
                    if (name.SequenceEqual("i")) {
                        return true;
                    } else if (name.SequenceEqual("indent")) {
                        return true;
                    }
                    break;
                case 'l':
                    if (name.SequenceEqual("line-height")) {
                        return true;
                    } else if (name.SequenceEqual("line-indent")) {
                        return true;
                    } else if (name.SequenceEqual("link")) {
                        return true;
                    } else if (name.SequenceEqual("lowercase")) {
                        return true;
                    }
                    break;
                case 'm':
                    if (name.SequenceEqual("margin")) {
                        return true;
                    } else if (name.SequenceEqual("mark")) {
                        return true;
                    } else if (name.SequenceEqual("mspace")) {
                        return true;
                    }
                    break;
                case 'n':
                    if (name.SequenceEqual("nobr")) {
                        return true;
                    } else if (name.SequenceEqual("noparse")) {
                        return true;
                    }
                    break;
                case 'p':
                    if (name.SequenceEqual("page")) {
                        return true;
                    } else if (name.SequenceEqual("pos")) {
                        return true;
                    }
                    break;
                case 'r':
                    if (name.SequenceEqual("rotate")) {
                        return true;
                    }
                    break;
                case 's':
                    if (name.SequenceEqual("s")) {
                        return true;
                    } else if (name.SequenceEqual("size")) {
                        return true;
                    } else if (name.SequenceEqual("smallcaps")) {
                        return true;
                    } else if (name.SequenceEqual("space")) {
                        return true;
                    } else if (name.SequenceEqual("sprite")) {
                        return true;
                    } else if (name.SequenceEqual("strikethrough")) {
                        return true;
                    } else if (name.SequenceEqual("style")) {
                        return true;
                    } else if (name.SequenceEqual("sub")) {
                        return true;
                    } else if (name.SequenceEqual("sup")) {
                        return true;
                    }
                    break;
                case 'u':
                    if (name.SequenceEqual("u")) {
                        return true;
                    } else if (name.SequenceEqual("uppercase")) {
                        return true;
                    }
                    break;
                case 'v':
                    if (name.SequenceEqual("voffset")) {
                        return true;
                    }
                    break;
                case 'w':
                    if (name.SequenceEqual("width")) {
                        return true;
                    }
                    break;
                case '/':
                    if (name.Length == 1) {
                        return false;
                    }
                    switch (name[1]) {
                        case 'a':
                            if (name.SequenceEqual("/align")) {
                                return true;
                            } else if (name.SequenceEqual("/allcaps")) {
                                return true;
                            } else if (name.SequenceEqual("/alpha")) {
                                return true;
                            }
                            break;
                        case 'b':
                            if (name.SequenceEqual("/b")) {
                                return true;
                            }
                            break;
                        case 'c':
                            if (name.SequenceEqual("/color")) {
                                return true;
                            } else if (name.SequenceEqual("/cspace")) {
                                return true;
                            }
                            break;
                        case 'f':
                            if (name.SequenceEqual("/font")) {
                                return true;
                            } else if (name.SequenceEqual("/font-weight")) {
                                return true;
                            }
                            break;
                        case 'g':
                            if (name.SequenceEqual("/gradient")) {
                                return true;
                            }
                            break;
                        case 'i':
                            if (name.SequenceEqual("/i")) {
                                return true;
                            } else if (name.SequenceEqual("/indent")) {
                                return true;
                            }
                            break;
                        case 'l':
                            if (name.SequenceEqual("/line-height")) {
                                return true;
                            } else if (name.SequenceEqual("/line-indent")) {
                                return true;
                            } else if (name.SequenceEqual("/link")) {
                                return true;
                            } else if (name.SequenceEqual("/lowercase")) {
                                return true;
                            }
                            break;
                        case 'm':
                            if (name.SequenceEqual("/margin")) {
                                return true;
                            } else if (name.SequenceEqual("/mark")) {
                                return true;
                            } else if (name.SequenceEqual("/mspace")) {
                                return true;
                            }
                            break;
                        case 'n':
                            if (name.SequenceEqual("/nobr")) {
                                return true;
                            } else if (name.SequenceEqual("/noparse")) {
                                return true;
                            }
                            break;
                        case 'r':
                            if (name.SequenceEqual("/rotate")) {
                                return true;
                            }
                            break;
                        case 's':
                            if (name.SequenceEqual("/s")) {
                                return true;
                            } else if (name.SequenceEqual("/size")) {
                                return true;
                            } else if (name.SequenceEqual("/smallcaps")) {
                                return true;
                            } else if (name.SequenceEqual("/strikethrough")) {
                                return true;
                            } else if (name.SequenceEqual("/style")) {
                                return true;
                            } else if (name.SequenceEqual("/sub")) {
                                return true;
                            } else if (name.SequenceEqual("/sup")) {
                                return true;
                            }
                            break;
                        case 'u':
                            if (name.SequenceEqual("/u")) {
                                return true;
                            } else if (name.SequenceEqual("/uppercase")) {
                                return true;
                            }
                            break;
                        case 'v':
                            if (name.SequenceEqual("/voffset")) {
                                return true;
                            }
                            break;
                        case 'w':
                            if (name.SequenceEqual("/width")) {
                                return true;
                            }
                            break;
                    }
                    break;
            }
            return false;
        }
    }
}
