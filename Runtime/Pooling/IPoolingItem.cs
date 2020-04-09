namespace TG.Core {
    /// <summary>
    /// Optional Interface for resetting objects pooled
    /// </summary>
	public interface IPoolingItem {
		void Reset();
	}
}