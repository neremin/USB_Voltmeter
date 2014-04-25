using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Hardcodet.Util.Dependencies;
using Voltmeter.Common;
using Voltmeter.Common.Extensions;

namespace Voltmeter.Binding
{

    public sealed class BindingManager : IBindingManager
    {
        [field: NonSerialized]
        readonly Lazy<ConcurrentDictionary<BindingKey, IBinding>> Bindings
           = new Lazy<ConcurrentDictionary<BindingKey, IBinding>>(LazyThreadSafetyMode.PublicationOnly);

        public void RemoveAll()
        {
            if (Bindings.IsValueCreated)
            {
                var bindingKeys = Bindings.Value.Keys.ToArray();
                foreach (var key in bindingKeys)
                {
                    IBinding binding;
                    if (Bindings.Value.TryRemove(key, out binding))
                    {
                        binding.Dispose();
                    }
                }
            }
        }

        public IBinding GetBinding<TTarget>(Expression<Func<TTarget>> target)
        {
            return Bindings.Value[new BindingKey(target.MemberInfo())];
        }

        public IBinding BindOneWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source, 
                Expression<Func<TTarget>> target, 
                Func<TSource, TTarget> converter = null,
                TTarget defaultValue = default(TTarget)
            )
        {
            var key = new BindingKey(target.MemberInfo());
            var binding = new LambdaBindingProxy(LambdaBinding.BindOneWay(source, target, converter, defaultValue));
            if (!Bindings.Value.TryAdd(key, binding))
            {
                binding.Dispose();
                throw new InvalidOperationException(string.Format("Свойство {0} уже подписано на обновления", key.Name));
            }

            return binding;
        }

        public IBinding BindTwoWay<TSource, TTarget>
            (
                Expression<Func<TSource>> source, 
                Expression<Func<TTarget>> target, 
                Func<TSource, TTarget> forwardConverter = null, 
                Func<TTarget, TSource> reverseConverter = null,
                TTarget defaultValue = default(TTarget)
            )
        {
            var key = new BindingKey(target.MemberInfo());
            var binding = new LambdaBindingProxy(LambdaBinding.BindTwoWay(source, target, forwardConverter, reverseConverter, defaultValue));
            if (!Bindings.Value.TryAdd(key, binding))
            {
                binding.Dispose();
                throw new InvalidOperationException(string.Format("Свойство {0} уже подписано на обновления", key.Name));
            }

            return binding;
        }

        public void RemoveBinding<TTarget>(Expression<Func<TTarget>> target)
        {
            var key = new BindingKey(target.MemberInfo());
            IBinding binding;
            if (Bindings.Value.TryRemove(key, out binding))
            {
                binding.Dispose();
            }
        }

        sealed class LambdaBindingProxy : IBinding
        {
            readonly LambdaBinding _binding;
            public LambdaBindingProxy(LambdaBinding binding)
            {
                Contract.Requires<ArgumentNullException>(binding != null);
                _binding = binding;
            }

            public void Dispose()
            {
                _binding.Dispose();
            }

            public void RefreshTarget()
            {
                _binding.RefreshTargetValue();
            }
        }

        [DebuggerDisplay("{Name}")]
        struct BindingKey : IEquatable<BindingKey>
        {
            readonly int ModuleToken;
            readonly int MemberToken;
            public readonly string Name;

            public BindingKey(MemberInfo memberInfo)
            {
                Contract.Requires<ArgumentNullException>(memberInfo != null);
                Contract.Requires<ArgumentException>(memberInfo is PropertyInfo);

                ModuleToken = memberInfo.Module.MetadataToken;
                MemberToken = memberInfo.MetadataToken;
                Name = string.Concat(memberInfo.DeclaringType.Name, ".", memberInfo.Name);
            }

            public bool Equals(BindingKey that)
            {
                return this.MemberToken == that.MemberToken &&
                       this.ModuleToken == that.ModuleToken;
            }

            public override bool Equals(object obj)
            {
                if (obj is BindingKey)
                {
                    return Equals((BindingKey)obj);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCodeCombiner.CombineHashCodes(
                    this.MemberToken,
                    this.ModuleToken);
            }
        }
    }
}
