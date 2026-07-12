namespace RandomDupes.Randomization
{
    internal sealed class RandomizedAppearance
    {
        internal RandomizedAppearance(
            KCompBuilder.BodyData bodyData,
            string nameDonorId,
            string hairDonorId,
            string faceDonorId,
            string bodyDonorId,
            string skinDonorId)
        {
            BodyData = bodyData;
            NameDonorId = nameDonorId;
            HairDonorId = hairDonorId;
            FaceDonorId = faceDonorId;
            BodyDonorId = bodyDonorId;
            SkinDonorId = skinDonorId;
        }

        internal KCompBuilder.BodyData BodyData { get; }

        internal string NameDonorId { get; }

        internal string HairDonorId { get; }

        internal string FaceDonorId { get; }

        internal string BodyDonorId { get; }

        internal string SkinDonorId { get; }
    }
}
