namespace Hex
{
    public struct HexCoordinate
    {
        public int Q { get; set; }
        public int R { get; set; }
        public int S { get; private set; }

        public HexCoordinate(int q, int r)
        {
            Q = q;
            R = r;
            S = -Q - R;
        }
    }
}