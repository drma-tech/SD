using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace SD.WEB.Core
{
    public static class ExtensionMethods
    {
        public static NameValueCollection QueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        public static string? QueryString(this NavigationManager navigationManager, string key)
        {
            return navigationManager.QueryString()[key];
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static TaskAwaiter<(A, B)> GetAwaiter<A, B>(this (Task<A>, Task<B>) tuple)
        {
            async Task<(A, B)> Combine()
            {
                var (task1, task2) = tuple;
                await Task.WhenAll(task1, task2);
                return (task1.Result, task2.Result);
            }
            return Combine().GetAwaiter();
        }

        public static TaskAwaiter<(A, B, C)> GetAwaiter<A, B, C>(this (Task<A>, Task<B>, Task<C>) tuple)
        {
            async Task<(A, B, C)> Combine()
            {
                var (task1, task2, task3) = tuple;
                await Task.WhenAll(task1, task2, task3);
                return (task1.Result, task2.Result, task3.Result);
            }
            return Combine().GetAwaiter();
        }

        public static TaskAwaiter<(A, B, C, D)> GetAwaiter<A, B, C, D>(this (Task<A>, Task<B>, Task<C>, Task<D>) tuple)
        {
            async Task<(A, B, C, D)> Combine()
            {
                var (task1, task2, task3, task4) = tuple;
                await Task.WhenAll(task1, task2, task3, task4);
                return (task1.Result, task2.Result, task3.Result, task4.Result);
            }
            return Combine().GetAwaiter();
        }

        public static string HideExternalLink(this string? link)
        {
            return $"/redirect?link={SimpleEncrypt(link)}";
        }

        public static string ShowExternalLink(this string? link)
        {
            return SimpleDecrypt(link);
        }

        public static string SimpleEncrypt(this string? url)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(url ?? ""));
        }

        public static string SimpleDecrypt(this string? obfuscatedUrl)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(obfuscatedUrl ?? ""));
        }
    }
}