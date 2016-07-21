namespace Rabscuttle.networking.commands {

    public enum CommandCode {
        ADMIN,
        AWAY,
        CONNECT,
        DIE,
        ERROR,
        INFO,
        INVITE,
        ISON,
        JOIN,
        KICK,
        KILL,
        LINKS,
        LIST,
        LUSERS,
        MODE,
        MOTD,
        NAMES,
        NICK,
        NJOIN,
        NOTICE,
        OPER,
        PART,
        PASS,
        PING,
        PONG,
        PRIVMSG,
        QUIT,
        REHASH,
        RESTART,
        SERVER,
        SERVICE,
        SERVLIST,
        SQUERY,
        SQUIRT,
        SQUIT,
        STATS,
        SUMMON,
        TIME,
        TOPIC,
        TRACE,
        USER,
        USERHOST,
        USERS,
        VERSION,
        WALLOPS,
        WHO,
        WHOIS,
        WHOWAS,
        DEFAULT
    }

    public enum ChannelMode {

    }

    public enum MemberCode {
        DEFAULT,                  //   unvoiced;
        VOICED = 0x043,           // + voiced; RFC1459
        HALF_OPERATOR = 0x37,     // % half operator; hybrid
        OPERATOR = 0x64,          // @ operator; RFC1459
        CREATOR = 0x64,           // @ creator; RFC2811
        SERVICE = 0x33,           // ! service; KineIRCd
        FOUNDER = 0x46            // . founder; tr-ircd
    }

    // taken from: https://raw.githubusercontent.com/ircdocs/irc-defs/gh-pages/_data/numerics.yaml
    public enum ReplyCode {
        RPL_WELCOME = 001, // The first message sent after client registration. The text used varies widely
        RPL_YOURHOST = 002, // Part of the post-registration greeting. Text varies widely. Also known as RPL_YOURHOSTIS (InspIRCd)
        RPL_CREATED = 003, // Part of the post-registration greeting. Text varies widely and &lt;date&gt; is returned in a human-readable format. Also known as RPL_SERVERCREATED (InspIRCd)
        RPL_MYINFO = 004, // Part of the post-registration greeting. Also known as RPL_SERVERVERSION (InspIRCd)
        RPL_ISUPPORT = 005, // Advertises features, limits, and protocol options that clients should be aware of. Also known as RPL_PROTOCTL (Bahamut, Unreal, Ultimate)
        RPL_MAP = 006, // CONFLICT
        RPL_MAPEND = 007, // CONFLICT Also known as RPL_ENDMAP (InspIRCd)
        RPL_SNOMASK = 008, // Server notice mask (hex). Also known as RPL_SNOMASKIS (InspIRCd)
        RPL_STATMEMTOT = 009, //
        RPL_BOUNCE = 010, // Sent to the client to redirect it to another server. Also known as RPL_REDIR
        RPL_YOURCOOKIE = 014, //
        RPL_MAP_D = 015, // CONFLICT
        RPL_MAPMORE = 016, // CONFLICT
        RPL_MAPEND_D = 017, // CONFLICT
        RPL_HELLO = 020, // Used by Rusnet to send the initial "Please wait while we process your connection" message, rather than a server-sent NOTICE.
        RPL_APASSWARN_SET = 030, //
        RPL_APASSWARN_SECRET = 031, //
        RPL_APASSWARN_CLEAR = 032, //
        RPL_YOURID = 042, // Also known as RPL_YOURUUID (InspIRCd)
        RPL_SAVENICK = 043, // Sent to the client when their nickname was forced to change due to a collision
        RPL_ATTEMPTINGJUNC = 050, //
        RPL_ATTEMPTINGREROUTE = 051, //
        RPL_REMOTEISUPPORT = 105, // Same format as RPL_ISUPPORT, but returned when the client is requesting information from a remote server instead of the server it is currently connected to
        RPL_TRACELINK = 200, // See RFC
        RPL_TRACECONNECTING = 201, // See RFC
        RPL_TRACEHANDSHAKE = 202, // See RFC
        RPL_TRACEUNKNOWN = 203, // See RFC
        RPL_TRACEOPERATOR = 204, // See RFC
        RPL_TRACEUSER = 205, // See RFC
        RPL_TRACESERVER = 206, // See RFC
        RPL_TRACESERVICE = 207, // See RFC
        RPL_TRACENEWTYPE = 208, // See RFC
        RPL_TRACECLASS = 209, // See RFC
        RPL_STATS = 210, // CONFLICT Used instead of having multiple stats numerics
        RPL_STATSLINKINFO = 211, // Reply to STATS (See RFC)
        RPL_STATSCOMMANDS = 212, // Reply to STATS (See RFC)
        RPL_STATSCLINE = 213, // Reply to STATS (See RFC)
        RPL_STATSNLINE = 214, // CONFLICT Reply to STATS (See RFC), Also known as RPL_STATSOLDNLINE (ircu, Unreal)
        RPL_STATSILINE = 215, // Reply to STATS (See RFC)
        RPL_STATSKLINE = 216, // Reply to STATS (See RFC)
        RPL_STATSQLINE = 217, // CONFLICT
        RPL_STATSYLINE = 218, // Reply to STATS (See RFC)
        RPL_ENDOFSTATS = 219, // End of RPL_STATS* list.
        RPL_STATSPLINE = 220, // CONFLICT
        RPL_UMODEIS = 221, // Information about a user's own modes. Some daemons have extended the mode command and certain modes take parameters (like channel modes).
        RPL_MODLIST = 222, // CONFLICT
        RPL_STATSELINE = 223, // CONFLICT
        RPL_STATSFLINE = 224, // CONFLICT
        RPL_STATSDLINE = 225, // CONFLICT
        RPL_STATSCOUNT = 226, // CONFLICT
        RPL_STATSGLINE = 227, // CONFLICT
        RPL_STATSQLINE_D = 228, // CONFLICT
        RPL_STATSSPAMF = 229, //
        RPL_STATSEXCEPTTKL = 230, //
        RPL_RULES = 232, // CONFLICT
        RPL_SERVLIST = 234, // A service entry in the service list
        RPL_SERVLISTEND = 235, // Termination of an RPL_SERVLIST list
        RPL_STATSVERBOSE = 236, // Verbose server list?
        RPL_STATSENGINE = 237, // Engine name?
        RPL_STATSFLINE_D = 238, // CONFLICT Feature lines?
        RPL_STATSIAUTH = 239, //
        RPL_STATSVLINE = 240, // CONFLICT
        RPL_STATSLLINE = 241, // Reply to STATS (See RFC)
        RPL_STATSUPTIME = 242, // Reply to STATS (See RFC)
        RPL_STATSOLINE = 243, // Reply to STATS (See RFC); The info field is an extension found in some IRC daemons, which returns info such as an e-mail address or the name/job of an operator
        RPL_STATSHLINE = 244, // Reply to STATS (See RFC)
        RPL_STATSSLINE = 245, // CONFLICT
        RPL_STATSSERVICE = 246, // CONFLICT
        RPL_STATSBLINE = 247, // CONFLICT
        RPL_STATSULINE = 248, // CONFLICT
        RPL_STATSULINE_D = 249, // CONFLICT Extension to RFC1459?
        RPL_STATSDLINE_D = 250, // CONFLICT
        RPL_LUSERCLIENT = 251, // Reply to LUSERS command, other versions exist (eg. RFC2812); Text may vary.
        RPL_LUSEROP = 252, // Reply to LUSERS command - Number of IRC operators online
        RPL_LUSERUNKNOWN = 253, // Reply to LUSERS command - Number of connections in an unknown/unregistered state
        RPL_LUSERCHANNELS = 254, // Reply to LUSERS command - Number of channels formed
        RPL_LUSERME = 255, // Reply to LUSERS command - Information about local connections; Text may vary.
        RPL_ADMINME = 256, // Start of an RPL_ADMIN* reply. In practise, the server parameter is often never given, and instead the last param contains the text 'Administrative info about <server>'. Newer daemons seem to follow the RFC and output the server's hostname in the last parameter, but also output the server name in the text as per traditional daemons.
        RPL_ADMINLOC1 = 257, // Reply to ADMIN command (Location, first line)
        RPL_ADMINLOC2 = 258, // Reply to ADMIN command (Location, second line)
        RPL_ADMINEMAIL = 259, // Reply to ADMIN command (E-mail address of administrator)
        RPL_TRACELOG = 261, // See RFC
        RPL_TRACEPING = 262, // CONFLICT Extension to RFC1459?
        RPL_TRYAGAIN = 263, // When a server drops a command without processing it, it MUST use this reply. The last param text changes, and commonly provides the client with more information about why the command could not be processed (such as rate-limiting). Also known as RPL_LOAD_THROTTLED and RPL_LOAD2HI, I'm presuming they do the same thing.
        RPL_USINGSSL = 264, //
        RPL_LOCALUSERS = 265, // Returns the number of clients currently and the maximum number of clients that have been connected directly to this server at one time, respectively. The two optional parameters are not always provided. Also known as RPL_CURRENT_LOCAL
        RPL_GLOBALUSERS = 266, // Returns the number of clients currently connected to the network, and the maximum number of clients ever connected to the network at one time, respectively. Also known as RPL_CURRENT_GLOBAL
        RPL_START_NETSTAT = 267, //
        RPL_NETSTAT = 268, //
        RPL_END_NETSTAT = 269, //
        RPL_PRIVS = 270, // CONFLICT
        RPL_SILELIST = 271, //
        RPL_ENDOFSILELIST = 272, //
        RPL_NOTIFY = 273, //
        RPL_ENDNOTIFY = 274, // CONFLICT
        RPL_STATSDLINE_D_D = 275, // CONFLICT
        RPL_WHOISCERTFP = 276, // CONFLICT Shows the SSL/TLS certificate fingerprint used by the client with the given nickname. Only sent when users <code>WHOIS</code> themselves or when an operator sends the <code>WHOIS</code>. Also adopted by hybrid 8.1 and charybdis 3.2
        RPL_GLIST = 280, //
        RPL_ENDOFGLIST = 281, // CONFLICT
        RPL_ENDOFACCEPT = 282, // CONFLICT
        RPL_ALIST = 283, // CONFLICT
        RPL_ENDOFALIST = 284, // CONFLICT
        RPL_GLIST_HASH = 285, // CONFLICT
        RPL_CHANINFO_USERS = 286, // CONFLICT
        RPL_CHANINFO_CHOPS = 287, // CONFLICT
        RPL_CHANINFO_VOICES = 288, // CONFLICT
        RPL_CHANINFO_AWAY = 289, // CONFLICT
        RPL_CHANINFO_OPERS = 290, // CONFLICT
        RPL_CHANINFO_BANNED = 291, // CONFLICT
        RPL_CHANINFO_BANS = 292, // CONFLICT
        RPL_CHANINFO_INVITE = 293, // CONFLICT
        RPL_CHANINFO_INVITES = 294, // CONFLICT
        RPL_CHANINFO_KICK = 295, // CONFLICT
        RPL_CHANINFO_KICKS = 296, //
        RPL_END_CHANINFO = 299, //
        RPL_NONE = 300, // Dummy reply, supposedly only used for debugging/testing new features, however has appeared in production daemons.
        RPL_AWAY = 301, // Used in reply to a command directed at a user who is marked as away
        RPL_USERHOST = 302, // Reply used by USERHOST (see RFC)
        RPL_ISON = 303, // Reply to the ISON command (see RFC)
        RPL_TEXT = 304, // CONFLICT Displays text to the user. This seems to have been defined in irc2.7h but never used. Servers generally use specific numerics or server notices instead of this. Unreal uses this numeric, but most others don't use it
        RPL_UNAWAY = 305, // Reply from AWAY when no longer marked as away
        RPL_NOWAWAY = 306, // Reply from AWAY when marked away
        RPL_USERIP = 307, // CONFLICT
        RPL_NOTIFYACTION = 308, // CONFLICT
        RPL_NICKTRACE = 309, // CONFLICT
        RPL_WHOISSVCMSG = 310, // CONFLICT
        RPL_WHOISUSER = 311, // Reply to WHOIS - Information about the user
        RPL_WHOISSERVER = 312, // Reply to WHOIS - What server they're on
        RPL_WHOISOPERATOR = 313, // Reply to WHOIS - User has IRC Operator privileges
        RPL_WHOWASUSER = 314, // Reply to WHOWAS - Information about the user
        RPL_ENDOFWHO = 315, // Used to terminate a list of RPL_WHOREPLY replies
        RPL_WHOISPRIVDEAF = 316, //
        RPL_WHOISIDLE = 317, // Reply to WHOIS - Idle information
        RPL_ENDOFWHOIS = 318, // Reply to WHOIS - End of list
        RPL_WHOISCHANNELS = 319, // Reply to WHOIS - Channel list for user (See RFC)
        RPL_WHOISVIRT = 320, // CONFLICT
        RPL_LIST = 322, // Channel list - A channel
        RPL_LISTEND = 323, // Channel list - End of list
        RPL_CHANNELMODEIS = 324, //
        RPL_UNIQOPIS = 325, // CONFLICT
        RPL_NOCHANPASS = 326, //
        RPL_CHPASSUNKNOWN = 327, // CONFLICT
        RPL_CHANNEL_URL = 328, // Also known as RPL_CHANNELURL in charybdis
        RPL_CREATIONTIME = 329, // Also known as RPL_CHANNELCREATED (InspIRCd)
        RPL_WHOWAS_TIME = 330, // CONFLICT
        RPL_NOTOPIC = 331, // Response to TOPIC when no topic is set. Also known as RPL_NOTOPICSET (InspIRCd)
        RPL_TOPIC = 332, // Response to TOPIC with the set topic. Also known as RPL_TOPICSET (InspIRCd)
        RPL_TOPICWHOTIME = 333, //
        RPL_LISTUSAGE = 334, // CONFLICT
        RPL_WHOISBOT = 335, // CONFLICT
        RPL_INVITELIST = 336, // CONFLICT Since hybrid 8.2.0. Not to be confused with the more widely used 346. A "list of channels a client is invited to" sent with /INVITE
        RPL_ENDOFINVITELIST = 337, // CONFLICT Since hybrid 8.2.0. Not to be confused with the more widely used 347.
        RPL_CHANPASSOK = 338, // CONFLICT
        RPL_BADCHANPASS = 339, // CONFLICT
        RPL_USERIP_D = 340, //
        RPL_INVITING = 341, // Returned by the server to indicate that the attempted INVITE message was successful and is being passed onto the end client. Note that RFC1459 documents the parameters in the reverse order. The format given here is the format used on production servers, and should be considered the standard reply above that given by RFC1459.
        RPL_WHOISKILL = 343, //
        RPL_INVITED = 345, // Sent to users on a channel when an INVITE command has been issued. Also known as RPL_ISSUEDINVITE (ircu)
        RPL_INVITELIST_D = 346, // An invite mask for the invite mask list. Also known as RPL_INVEXLIST in hybrid 8.2.0
        RPL_ENDOFINVITELIST_D = 347, // Termination of an RPL_INVITELIST list. Also known as RPL_ENDOFINVEXLIST in hybrid 8.2.0
        RPL_EXCEPTLIST = 348, // An exception mask for the exception mask list. Also known as RPL_EXLIST (Unreal, Ultimate)
        RPL_ENDOFEXCEPTLIST = 349, // Termination of an RPL_EXCEPTLIST list. Also known as RPL_ENDOFEXLIST (Unreal, Ultimate)
        RPL_VERSION = 351, // Reply by the server showing its version details, however this format is not often adhered to
        RPL_WHOREPLY = 352, // Reply to vanilla WHO (See RFC). This format can be very different if the 'WHOX' version of the command is used (see ircu).
        RPL_NAMREPLY = 353, // Reply to NAMES (See RFC)
        RPL_WHOSPCRPL = 354, // Reply to WHO, however it is a 'special' reply because it is returned using a non-standard (non-RFC1459) format. The format is dictated by the command given by the user, and can vary widely. When this is used, the WHO command was invoked in its 'extended' form, as announced by the 'WHOX' ISUPPORT tag.
        RPL_NAMREPLY_ = 355, // Reply to the \users (when the channel is set +D, QuakeNet relative). The proper define name for this numeric is unknown at this time. Also known as RPL_DELNAMREPLY (ircu)
        RPL_MAP_D_D = 357, // CONFLICT
        RPL_MAPMORE_D = 358, // CONFLICT
        RPL_MAPEND_D_D = 359, // CONFLICT
        RPL_LINKS = 364, // Reply to the LINKS command
        RPL_ENDOFLINKS = 365, // Termination of an RPL_LINKS list
        RPL_ENDOFNAMES = 366, // Termination of an RPL_NAMREPLY list
        RPL_BANLIST = 367, // A ban-list item (See RFC); <time left> and <reason> are additions used by various servers.
        RPL_ENDOFBANLIST = 368, // Termination of an RPL_BANLIST list
        RPL_ENDOFWHOWAS = 369, // Reply to WHOWAS - End of list
        RPL_INFO = 371, // Reply to INFO
        RPL_MOTD = 372, // Reply to MOTD
        RPL_ENDOFINFO = 374, // Termination of an RPL_INFO list
        RPL_MOTDSTART = 375, // Start of an RPL_MOTD list
        RPL_ENDOFMOTD = 376, // Termination of an RPL_MOTD list
        RPL_KICKEXPIRED = 377, // CONFLICT
        RPL_BANEXPIRED = 378, // CONFLICT
        RPL_KICKLINKED = 379, // CONFLICT
        RPL_BANLINKED = 380, // CONFLICT
        RPL_YOUREOPER = 381, // Successful reply from OPER. Also known asRPL_YOUAREOPER (InspIRCd)
        RPL_REHASHING = 382, // Successful reply from REHASH
        RPL_YOURESERVICE = 383, // Sent upon successful registration of a service
        RPL_NOTOPERANYMORE = 385, //
        RPL_QLIST = 386, // CONFLICT
        RPL_ENDOFQLIST = 387, // CONFLICT
        RPL_ALIST_D = 388, // CONFLICT
        RPL_ENDOFALIST_D = 389, //
        RPL_TIME = 391, // Response to the TIME command. The string format may vary greatly.
        RPL_USERSSTART = 392, // Start of an RPL_USERS list
        RPL_USERS = 393, // Response to the USERS command (See RFC)
        RPL_ENDOFUSERS = 394, // Termination of an RPL_USERS list
        RPL_NOUSERS = 395, // Reply to USERS when nobody is logged in
        RPL_HOSTHIDDEN = 396, // CONFLICT Reply to a user when user mode +x (host masking) was set successfully
        ERR_UNKNOWNERROR = 400, // Sent when an error occured executing a command, but it is not specifically known why the command could not be executed.
        ERR_NOSUCHNICK = 401, // Used to indicate the nickname parameter supplied to a command is currently unused
        ERR_NOSUCHSERVER = 402, // Used to indicate the server name given currently doesn't exist
        ERR_NOSUCHCHANNEL = 403, // Used to indicate the given channel name is invalid, or does not exist
        ERR_CANNOTSENDTOCHAN = 404, // Sent to a user who does not have the rights to send a message to a channel
        ERR_TOOMANYCHANNELS = 405, // Sent to a user when they have joined the maximum number of allowed channels and they tried to join another channel
        ERR_WASNOSUCHNICK = 406, // Returned by WHOWAS to indicate there was no history information for a given nickname
        ERR_TOOMANYTARGETS = 407, // The given target(s) for a command are ambiguous in that they relate to too many targets
        ERR_NOSUCHSERVICE = 408, // Returned to a client which is attempting to send an SQUERY (or other message) to a service which does not exist
        ERR_NOORIGIN = 409, // PING or PONG message missing the originator parameter which is required since these commands must work without valid prefixes
        ERR_INVALIDCAPCMD = 410, // Returned when a client sends a CAP subcommand which is invalid or otherwise issues an invalid CAP command. Also known as ERR_INVALIDCAPSUBCOMMAND (InspIRCd) or ERR_UNKNOWNCAPCMD (ircu)
        ERR_NORECIPIENT = 411, // Returned when no recipient is given with a command
        ERR_NOTEXTTOSEND = 412, // Returned when NOTICE/PRIVMSG is used with no message given
        ERR_NOTOPLEVEL = 413, // Used when a message is being sent to a mask without being limited to a top-level domain (i.e. * instead of *.au)
        ERR_WILDTOPLEVEL = 414, // Used when a message is being sent to a mask with a wild-card for a top level domain (i.e. *.*)
        ERR_BADMASK = 415, // Used when a message is being sent to a mask with an invalid syntax
        ERR_TOOMANYMATCHES = 416, // Returned when too many matches have been found for a command and the output has been truncated. An example would be the WHO command, where by the mask '*' would match everyone on the network! Ouch!
        ERR_INPUTTOOLONG = 417, // Returned when an input line is longer than the server can process (512 bytes), to let the client know this line was dropped (rather than being truncated)
        ERR_LENGTHTRUNCATED = 419, //
        ERR_UNKNOWNCOMMAND = 421, // Returned when the given command is unknown to the server (or hidden because of lack of access rights)
        ERR_NOMOTD = 422, // Sent when there is no MOTD to send the client
        ERR_NOADMININFO = 423, // Returned by a server in response to an ADMIN request when no information is available. RFC1459 mentions this in the list of numerics. While it's not listed as a valid reply in section 4.3.7 ('Admin command'), it's confirmed to exist in the real world.
        ERR_FILEERROR = 424, // Generic error message used to report a failed file operation during the processing of a command
        ERR_NOOPERMOTD = 425, //
        ERR_TOOMANYAWAY = 429, //
        ERR_EVENTNICKCHANGE = 430, // Returned by NICK when the user is not allowed to change their nickname due to a channel event (channel mode +E)
        ERR_NONICKNAMEGIVEN = 431, // Returned when a nickname parameter expected for a command isn't found
        ERR_ERRONEUSNICKNAME = 432, // Returned after receiving a NICK message which contains a nickname which is considered invalid, such as it's reserved ('anonymous') or contains characters considered invalid for nicknames. This numeric is misspelt, but remains with this name for historical reasons :)
        ERR_NICKNAMEINUSE = 433, // Returned by the NICK command when the given nickname is already in use
        ERR_SERVICENAMEINUSE = 434, // CONFLICT
        ERR_SERVICECONFUSED = 435, // CONFLICT
        ERR_NICKCOLLISION = 436, // Returned by a server to a client when it detects a nickname collision
        ERR_UNAVAILRESOURCE = 437, // CONFLICT Return when the target is unable to be reached temporarily, eg. a delay mechanism in play, or a service being offline
        ERR_NICKTOOFAST = 438, // CONFLICT Also known as ERR_NCHANGETOOFAST (Unreal, Ultimate)
        ERR_TARGETTOOFAST = 439, // Also known as many other things, RPL_INVTOOFAST, RPL_MSGTOOFAST etc
        ERR_SERVICESDOWN = 440, //
        ERR_USERNOTINCHANNEL = 441, // Returned by the server to indicate that the target user of the command is not on the given channel
        ERR_NOTONCHANNEL = 442, // Returned by the server whenever a client tries to perform a channel effecting command for which the client is not a member
        ERR_USERONCHANNEL = 443, // Returned when a client tries to invite a user to a channel they're already on
        ERR_NOLOGIN = 444, // Returned by the SUMMON command if a given user was not logged in and could not be summoned
        ERR_SUMMONDISABLED = 445, // Returned by SUMMON when it has been disabled or not implemented
        ERR_USERSDISABLED = 446, // Returned by USERS when it has been disabled or not implemented
        ERR_NONICKCHANGE = 447, // This numeric is called ERR_CANTCHANGENICK in InspIRCd
        ERR_FORBIDDENCHANNEL = 448, // Returned when this channel name has been explicitly blocked and is not allowed to be used.
        ERR_NOTIMPLEMENTED = 449, // Returned when a requested feature is not implemented (and cannot be completed)
        ERR_NOTREGISTERED = 451, // Returned by the server to indicate that the client must be registered before the server will allow it to be parsed in detail
        ERR_IDCOLLISION = 452, //
        ERR_NICKLOST = 453, //
        ERR_HOSTILENAME = 455, //
        ERR_ACCEPTFULL = 456, //
        ERR_ACCEPTEXIST = 457, //
        ERR_ACCEPTNOT = 458, //
        ERR_NOHIDING = 459, // Not allowed to become an invisible operator?
        ERR_NOTFORHALFOPS = 460, //
        ERR_NEEDMOREPARAMS = 461, // Returned by the server by any command which requires more parameters than the number of parameters given
        ERR_ALREADYREGISTERED = 462, // Returned by the server to any link which attempts to register again Also known as ERR_ALREADYREGISTRED (sic) in ratbox/charybdis.
        ERR_NOPERMFORHOST = 463, // Returned to a client which attempts to register with a server which has been configured to refuse connections from the client's host
        ERR_PASSWDMISMATCH = 464, // Returned by the PASS command to indicate the given password was required and was either not given or was incorrect
        ERR_YOUREBANNEDCREEP = 465, // Returned to a client after an attempt to register on a server configured to ban connections from that client
        ERR_KEYSET = 467, // Returned when the channel key for a channel has already been set
        ERR_INVALIDUSERNAME = 468, // CONFLICT
        ERR_LINKSET = 469, //
        ERR_LINKCHANNEL = 470, // CONFLICT
        ERR_CHANNELISFULL = 471, // Returned when attempting to join a channel which is set +l and is already full
        ERR_UNKNOWNMODE = 472, // Returned when a given mode is unknown
        ERR_INVITEONLYCHAN = 473, // Returned when attempting to join a channel which is invite only without an invitation
        ERR_BANNEDFROMCHAN = 474, // Returned when attempting to join a channel a user is banned from
        ERR_BADCHANNELKEY = 475, // Returned when attempting to join a key-locked channel either without a key or with the wrong key
        ERR_BADCHANMASK = 476, // The given channel mask was invalid
        ERR_NOCHANMODES = 477, // CONFLICT Returned when attempting to set a mode on a channel which does not support channel modes, or channel mode changes. Also known as ERR_MODELESS
        ERR_BANLISTFULL = 478, // Returned when a channel access list (i.e. ban list etc) is full and cannot be added to
        ERR_BADCHANNAME = 479, // CONFLICT Returned to indicate that a given channel name is not valid. Servers that implement this use it instead of `ERR_NOSUCHCHANNEL` where appropriate.
        ERR_NOULINE = 480, // CONFLICT
        ERR_NOPRIVILEGES = 481, // Returned by any command requiring special privileges (eg. IRC operator) to indicate the operation was unsuccessful
        ERR_CHANOPRIVSNEEDED = 482, // Returned by any command requiring special channel privileges (eg. channel operator) to indicate the operation was unsuccessful. InspIRCd also uses this numeric "for other things like trying to kick a uline"
        ERR_CANTKILLSERVER = 483, // Returned by KILL to anyone who tries to kill a server
        ERR_RESTRICTED = 484, // CONFLICT Sent by the server to a user upon connection to indicate the restricted nature of the connection (i.e. usermode +r)
        ERR_UNIQOPRIVSNEEDED = 485, // Any mode requiring 'channel creator' privileges returns this error if the client is attempting to use it while not a channel creator on the given channel
        ERR_NONONREG = 486, // CONFLICT
        ERR_CHANTOORECENT = 487, // CONFLICT
        ERR_TSLESSCHAN = 488, // CONFLICT
        ERR_SECUREONLYCHAN = 489, // CONFLICT Also known as ERR_SSLONLYCHAN.
        ERR_ALLMUSTSSL = 490, // CONFLICT
        ERR_NOOPERHOST = 491, // Returned by OPER to a client who cannot become an IRC operator because the server has been configured to disallow the client's host
        ERR_NOCTCP = 492, // CONFLICT Notifies the user that a message they have sent to a channel has been rejected as it contains CTCPs, and they cannot send messages containing CTCPs to this channel
        ERR_NOFEATURE = 493, //
        ERR_BADFEATVALUE = 494, // CONFLICT
        ERR_BADLOGTYPE = 495, // CONFLICT
        ERR_BADLOGSYS = 496, //
        ERR_BADLOGVALUE = 497, //
        ERR_ISOPERLCHAN = 498, //
        ERR_CHANOWNPRIVNEEDED = 499, // Works just like ERR_CHANOPRIVSNEEDED except it indicates that owner status (+q) is needed.
        ERR_TOOMANYJOINS = 500, // CONFLICT
        ERR_UMODEUNKNOWNFLAG = 501, // Returned by the server to indicate that a MODE message was sent with a nickname parameter and that the mode flag sent was not recognised
        ERR_USERSDONTMATCH = 502, // Error sent to any user trying to view or change the user mode for a user other than themselves
        ERR_USERNOTONSERV = 504, //
        ERR_SILELISTFULL = 511, //
        ERR_TOOMANYWATCH = 512, // CONFLICT Also known as ERR_NOTIFYFULL (aircd), I presume they are the same
        ERR_BADPING = 513, // Also known as ERR_NEEDPONG (Unreal/Ultimate) for use during registration, however it's not used in Unreal (and might not be used in Ultimate either). Also known as ERR_WRONGPONG (Ratbox/charybdis)
        ERR_TOOMANYDCC = 514, // CONFLICT
        ERR_BADEXPIRE = 515, //
        ERR_DONTCHEAT = 516, //
        ERR_DISABLED = 517, //
        ERR_NOINVITE = 518, // CONFLICT
        ERR_ADMONLY = 519, // CONFLICT
        ERR_OPERONLY = 520, // CONFLICT Also known as ERR_OPERONLYCHAN (Hybrid) and ERR_CANTJOINOPERSONLY (InspIRCd).
        ERR_LISTSYNTAX = 521, // CONFLICT
        ERR_WHOSYNTAX = 522, //
        ERR_WHOLIMEXCEED = 523, //
        ERR_QUARANTINED = 524, // CONFLICT
        ERR_INVALIDKEY = 525, //
        ERR_CANTSENDTOUSER = 531, //
        ERR_BADHOSTMASK = 550, //
        ERR_HOSTUNAVAIL = 551, //
        ERR_USINGSLINE = 552, //
        ERR_STATSSLINE = 553, // CONFLICT
        ERR_NOTLOWEROPLEVEL = 560, //
        ERR_NOTMANAGER = 561, //
        ERR_CHANSECURED = 562, //
        ERR_UPASSSET = 563, //
        ERR_UPASSNOTSET = 564, //
        ERR_NOMANAGER = 566, //
        ERR_UPASS_SAME_APASS = 567, //
        ERR_LASTERROR = 568, // CONFLICT
        RPL_REAWAY = 597, //
        RPL_GONEAWAY = 598, // Used when adding users to their <code>WATCH</code> list.
        RPL_NOTAWAY = 599, // Used when adding users to their <code>WATCH</code> list.
        RPL_LOGON = 600, //
        RPL_LOGOFF = 601, //
        RPL_WATCHOFF = 602, //
        RPL_WATCHSTAT = 603, //
        RPL_NOWON = 604, //
        RPL_NOWOFF = 605, //
        RPL_WATCHLIST = 606, //
        RPL_ENDOFWATCHLIST = 607, //
        RPL_WATCHCLEAR = 608, // Also known as RPL_CLEARWATCH in Unreal
        RPL_NOWISAWAY = 609, // Returned when adding users to their <code>WATCH</code> list.
        RPL_MAPMORE_D_D = 610, // CONFLICT
        RPL_ISLOCOP = 611, //
        RPL_ISNOTOPER = 612, //
        RPL_ENDOFISOPER = 613, //
        RPL_MAPMORE_D_D_D = 615, // CONFLICT
        RPL_WHOISHOST = 616, // CONFLICT
        RPL_WHOISSSLFP = 617, // CONFLICT
        RPL_DCCLIST = 618, //
        RPL_ENDOFDCCLIST = 619, // CONFLICT
        RPL_DCCINFO = 620, // CONFLICT
        RPL_RULES_D = 621, // CONFLICT
        RPL_ENDOFRULES = 622, // CONFLICT
        RPL_MAPMORE_D_D_D_D = 623, // CONFLICT
        RPL_OMOTDSTART = 624, //
        RPL_OMOTD = 625, //
        RPL_ENDOFOMOTD_D = 626, //
        RPL_SETTINGS = 630, //
        RPL_ENDOFSETTINGS = 631, //
        RPL_SPAMCMDFWD = 659, // Used to let a client know that a copy of their command has been passed to operators and the reason for it.
        RPL_STARTTLS = 670, // Indicates that the client may begin the TLS handshake
        RPL_WHOISSECURE = 671, // The text in the last parameter may change. Also known as RPL_WHOISSSL (Nefarious).
        RPL_UNKNOWNMODES = 672, // CONFLICT Returns a full list of modes that are unknown when a client issues a MODE command (rather than one numeric per mode)
        RPL_CANNOTSETMODES = 673, // Returns a full list of modes that cannot be set when a client issues a MODE command
        ERR_STARTTLS = 691, // Indicates that a server-side error has occured
        RPL_MODLIST_D = 702, // CONFLICT Output from the MODLIST command
        RPL_ENDOFMODLIST = 703, // CONFLICT Terminates MODLIST output
        RPL_HELPSTART = 704, // Start of HELP command output
        RPL_HELPTXT = 705, // Output from HELP command
        RPL_ENDOFHELP = 706, // End of HELP command output
        ERR_TARGCHANGE = 707, // See doc/tgchange.txt in the charybdis source.
        RPL_ETRACEFULL = 708, // Output from 'extended' trace
        RPL_ETRACE = 709, // Output from 'extended' trace
        RPL_KNOCK = 710, // Message delivered using KNOCK command
        RPL_KNOCKDLVR = 711, // Message returned from using KNOCK command (KNOCK delivered)
        ERR_TOOMANYKNOCK = 712, // Message returned when too many KNOCKs for a channel have been sent by a user
        ERR_CHANOPEN = 713, // Message returned from KNOCK when the channel can be freely joined by the user
        ERR_KNOCKONCHAN = 714, // Message returned from KNOCK when the user has used KNOCK on a channel they have already joined
        ERR_KNOCKDISABLED = 715, // CONFLICT Returned from KNOCK when the command has been disabled
        RPL_TARGUMODEG = 716, // Sent to indicate the given target is set +g (server-side ignore) Mentioned as RPL_TARGUMODEG in the CALLERID spec, ERR_TARGUMODEG in the ratbox/charybdis implementations.
        RPL_TARGNOTIFY = 717, // Sent following a PRIVMSG/NOTICE to indicate the target has been notified of an attempt to talk to them while they are set +g
        RPL_UMODEGMSG = 718, // Sent to a user who is +g to inform them that someone has attempted to talk to them (via PRIVMSG/NOTICE), and that they will need to be accepted (via the ACCEPT command) before being able to talk to them
        RPL_OMOTDSTART_D = 720, // IRC Operator MOTD header, sent upon OPER command
        RPL_OMOTD_D = 721, // IRC Operator MOTD text (repeated, usually)
        RPL_ENDOFOMOTD = 722, // IRC operator MOTD footer
        ERR_NOPRIVS = 723, // Returned from an oper command when the IRC operator does not have the relevant operator privileges.
        RPL_TESTMASK = 724, // Reply from an oper command reporting how many users match a given user@host mask
        RPL_TESTLINE = 725, // Reply from an oper command reporting relevant I/K lines that will match a given user@host
        RPL_NOTESTLINE = 726, // Reply from oper command reporting no I/K lines match the given user@host
        RPL_TESTMASKGECOS = 727, // From the m_testmask module, "Shows the number of matching local and global clients for a user@host mask"
        RPL_QUIETLIST = 728, // Same thing as RPL_BANLIST, but for mode +q (quiet)
        RPL_ENDOFQUIETLIST = 729, // Same thing as RPL_ENDOFBANLIST, but for mode +q (quiet)
        RPL_MONONLINE = 730, // Used to indicate to a client that either a target has just become online, or that a target they have added to their monitor list is online
        RPL_MONOFFLINE = 731, // Used to indicate to a client that either a target has just left the IRC network, or that a target they have added to their monitor list is offline
        RPL_MONLIST = 732, // Used to indicate to a client the list of targets they have in their monitor list
        RPL_ENDOFMONLIST = 733, // Used to indicate to a client the end of a monitor list
        ERR_MONLISTFULL = 734, // Used to indicate to a client that their monitor list is full, so the MONITOR command failed
        RPL_RSACHALLENGE2 = 740, // From the ratbox m_challenge module, to auth opers.
        RPL_ENDOFRSACHALLENGE2 = 741, // From the ratbox m_challenge module, to auth opers.
        ERR_MLOCKRESTRICTED = 742, //
        ERR_INVALIDBAN = 743, //
        ERR_TOPICLOCK = 744, // Defined in the Charybdis source code with the comment <code>/* inspircd */</code>
        RPL_SCANMATCHED = 750, // From the ratbox m_scan module.
        RPL_SCANUMODES = 751, // From the ratbox m_scan module.
        RPL_WHOISKEYVALUE = 760, // Reply to WHOIS - Metadata key/value associated with the target
        RPL_KEYVALUE = 761, // Returned to show a currently set metadata key and its value, or a metadata key that has been cleared if no value is present in the response
        RPL_METADATAEND = 762, // Indicates the end of a list of metadata keys
        ERR_METADATALIMIT = 764, // Used to indicate to a client that their metadata store is full, and they cannot add the requested key(s)
        ERR_TARGETINVALID = 765, // Indicates to a client that the target of a sent METADATA command is invalid
        ERR_NOMATCHINGKEY = 766, // Indicates to a client that the requested metadata key does not exist
        ERR_KEYINVALID = 767, // Indicates to a client that the requested metadata key is not valid
        ERR_KEYNOTSET = 768, // Indicates to a client that the metadata key they requested to clear is not already set
        ERR_KEYNOPERMISSION = 769, // Indicates to a client that they do not have permission to set the requested metadata key
        RPL_XINFO = 771, // Used to send 'eXtended info' to the client, a replacement for the STATS command to send a large variety of data and minimise numeric pollution.
        RPL_XINFOSTART = 773, // Start of an RPL_XINFO list
        RPL_XINFOEND = 774, // Termination of an RPL_XINFO list
        RPL_LOGGEDIN = 900, // Sent when the user�s account name is set (whether by SASL or otherwise)
        RPL_LOGGEDOUT = 901, // Sent when the user�s account name is unset (whether by SASL or otherwise)
        ERR_NICKLOCKED = 902, // Sent when the SASL authentication fails because the account is currently locked out, held, or otherwise administratively made unavailable.
        RPL_SASLSUCCESS = 903, // Sent when the SASL authentication finishes successfully
        ERR_SASLFAIL = 904, // Sent when the SASL authentication fails because of invalid credentials or other errors not explicitly mentioned by other numerics
        ERR_SASLTOOLONG = 905, // Sent when credentials are valid, but the SASL authentication fails because the client-sent AUTHENTICATE command was too long (i.e. the parameter longer than 400 bytes)
        ERR_SASLABORTED = 906, // Sent when the SASL authentication is aborted because the client sent an AUTHENTICATE command with * as the parameter
        ERR_SASLALREADY = 907, // Sent when the client attempts to initiate SASL authentication after it has already finished successfully for that connection.
        RPL_SASLMECHS = 908, // Sent when the client requests a list of SASL mechanisms supported by the server (or network, services). The numeric contains a comma-separated list of mechanisms
        ERR_WORDFILTERED = 936, //
        ERR_CANNOTDOCOMMAND = 972, // CONFLICT Indicates that a command could not be performed for an arbitrary reason. For example, a halfop trying to kick an op.
        RPL_UNLOADEDMODULE = 973, //
        ERR_CANNOTCHANGECHANMODE = 974, // CONFLICT Indicates that a channel mode could not be changeded for an arbitrary reason. For instance, trying to set OPER_ONLY when you are not an IRC operator.
        RPL_LOADEDMODULE = 975, // CONFLICT
        ERR_NUMERIC_ERR = 999, // Also known as ERR_NUMERICERR (Unreal) or ERR_LAST_ERR_MSG
    }
}
