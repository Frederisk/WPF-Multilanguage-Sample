using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MultiLanguage.Common {
    /// </summary>
    /// <remarks>
    /// https://github.com/Microsoft/Windows-appsample-networkhelper
    /// </remarks>
    public abstract class BindableBase : INotifyPropertyChanged {
        /// <summary>
        /// 屬性更改通知的多播事件。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// 檢查屬性是否已與所需值匹配。 設置屬性, 並在必要時通知攔截器。
        /// </summary>
        /// <typeparam name="T">屬性的類型。</typeparam>
        /// <param name="storage">對具有 getter 和 setter 的屬性的引用。</param>
        /// <param name="value">屬性所需的值。</param>
        /// <param name="propertyName">用於通知攔截器的屬性的名稱。
        /// 此值是可選的, 可以在支援 CallerMemberName 的編譯器調用時自動提供。</param>
        /// <returns>如果值更改為 True, 則如果現有值與所需值匹配, 則為 false。</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String? propertyName = null) {
            if (Object.Equals(storage, value)) {
                return false;
            }
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// 通知攔截器屬性值已更改。
        /// </summary>
        /// <param name="propertyName">用於通知攔截器的屬性的名稱。
        /// 此值是可選的, 可以在支援 <see cref="CallerMemberNameAttribute"/>
        /// 的編譯器中調用時自動提供。</param>
        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}