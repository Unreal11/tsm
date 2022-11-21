using Core.Contracts;

namespace Mirror.Extensions
{
    public static class ReaderWriterExtensions
    {
        public static void WriteAuthDto(this NetworkWriter writer, AuthDto authDto)
        {
            writer.WriteString(authDto.Login);
            writer.WriteString(authDto.Password);
            writer.WriteBool(authDto.IsGuest);
        }

        public static AuthDto ReadAuthDto(this NetworkReader reader)
        {
            return new AuthDto
            {
                Login = reader.ReadString(),
                Password = reader.ReadString(),
                IsGuest = reader.ReadBool()
            };
        }

        public static void WriteAccountDto(this NetworkWriter writer, AccountDto accountDto)
        {
            writer.WriteGuid(accountDto.Id);
            writer.WriteString(accountDto.Login);
            writer.WriteBool(accountDto.IsGuest);
        }

        public static AccountDto ReadAccountDto(this NetworkReader reader)
        {
            return new AccountDto
            {
                Id = reader.ReadGuid(),
                Login = reader.ReadString(),
                IsGuest = reader.ReadBool()
            };
        }

        public static void WriteRequestMatchDto(this NetworkWriter writer, RequestMatchDto requestMatchDto)
        {
            writer.WriteGuid(requestMatchDto.AccountId);
            writer.WriteInt((int)requestMatchDto.RequestType);
        }

        public static RequestMatchDto ReadRequestMatchDto(this NetworkReader reader)
        {
            return new RequestMatchDto
            {
                AccountId = reader.ReadGuid(),
                RequestType = (MatchRequestType)reader.ReadInt()
            };
        }

        public static void WriteRequestCardDto(this NetworkWriter writer, RequestCardDto requestCardDto)
        {
            writer.WriteGuid(requestCardDto.AccountId);
            writer.WriteGuid(requestCardDto.CardId);
            writer.WriteInt((int)requestCardDto.ActionType);
        }

        public static RequestCardDto ReadRequestCardDto(this NetworkReader reader)
        {
            return new RequestCardDto
            {
                AccountId = reader.ReadGuid(),
                CardId = reader.ReadGuid(),
                ActionType = (CardActionType)reader.ReadInt()
            };
        }

        public static void WriteRequestBattleInfo(this NetworkWriter writer, RequestBattleInfo requestBattleInfo)
        {
            writer.WriteGuid(requestBattleInfo.AccountId);
            writer.WriteString(requestBattleInfo.EnemyName);
            writer.WriteString(requestBattleInfo.YourName);
            writer.WriteInt(requestBattleInfo.YourTowerHealth);
            writer.WriteInt(requestBattleInfo.EnemyTowerHealth);
            writer.WriteInt(requestBattleInfo.YourWallHealth);
            writer.WriteInt(requestBattleInfo.EnemyWallHealth);
            writer.WriteBool(requestBattleInfo.IsYourTurn);
            writer.WriteInt(requestBattleInfo.Timer);
            writer.WriteInt(requestBattleInfo.TurnFatigue);
            writer.WriteInt(requestBattleInfo.StartDamageFatigue);
            writer.WriteInt(requestBattleInfo.FatigueLimit);
        }

        public static RequestBattleInfo ReadRequestBattleInfo(this NetworkReader reader)
        {
            return new RequestBattleInfo
            {
                AccountId = reader.ReadGuid(),
                EnemyName = reader.ReadString(),
                YourName = reader.ReadString(),
                YourTowerHealth = reader.ReadInt(),
                EnemyTowerHealth = reader.ReadInt(),
                YourWallHealth = reader.ReadInt(),
                EnemyWallHealth = reader.ReadInt(),
                IsYourTurn = reader.ReadBool(),
                Timer = reader.ReadInt(),
                TurnFatigue = reader.ReadInt(),
                StartDamageFatigue = reader.ReadInt(),
                FatigueLimit = reader.ReadInt()
            };
        }

        public static void WritePinger(this NetworkWriter writer, Pinger pinger)
        {
            writer.WriteGuid(pinger.AccountId);
        }

        public static Pinger ReadPinger(this NetworkReader reader)
        {
            return new Pinger
            {
                AccountId = reader.ReadGuid()
            };
        }
    }
}