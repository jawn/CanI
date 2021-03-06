using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CanI.Core.Cleaners;
using CanI.Core.Configuration;
using CanI.Core.Predicates;

namespace CanI.Core.Authorization
{
    public class StringBasedPermission : Permission
    {
        private string AllowedAction { get; set; }
        private string AllowedSubject { get; set; }
        private readonly ActionCleaner actionCleaner;
        private readonly SubjectCleaner subjectCleaner;
        private readonly IList<IAuthorizationPredicate> authorizationPredicates;

        public StringBasedPermission(string action, string subject, ActionCleaner actionCleaner, SubjectCleaner subjectCleaner)
        {
            this.actionCleaner = actionCleaner;
            this.subjectCleaner = subjectCleaner;
            AllowedAction = actionCleaner.Clean(action);
            AllowedSubject = subjectCleaner.Clean(subject);

            authorizationPredicates = new List<IAuthorizationPredicate>();
        }

        public override IPermissionConfiguration If<T>(Func<T, bool> predicate)
        {
            authorizationPredicates.Add(new GenericPredicate<T>(predicate));
            return this;
        }

        public override bool Authorizes(string requestedAction, object requestedSubject)
        {
            return Matches(requestedAction, requestedSubject)
                   && ContextAllowsAction(requestedSubject)
                   && SubjectAllowsAction(requestedAction, requestedSubject);
        }

        private bool Matches(string action, object subject)
        {
            var requestedSubject = subjectCleaner.Clean(subject);
            var requestedAction = actionCleaner.Clean(action);

            return Regex.IsMatch(requestedSubject, AllowedSubject, RegexOptions.IgnoreCase)
                   && Regex.IsMatch(requestedAction, AllowedAction, RegexOptions.IgnoreCase);
        }

        private bool ContextAllowsAction(object requestedSubject)
        {
            return authorizationPredicates.All(p => p.Allows(requestedSubject));
        }

        private bool SubjectAllowsAction(string requestedAction, object requestedSubject)
        {
            const BindingFlags caseInsensitivePublicInstance = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
            var property = requestedSubject.GetType().GetProperty("can" + PureAction(requestedAction), caseInsensitivePublicInstance);
            if (property == null) return true;

            var propertyValue = property.GetValue(requestedSubject, BindingFlags.Instance, null, null, null);

            var booleanValue = propertyValue as bool?;
            return booleanValue.GetValueOrDefault();
        }

        private static string PureAction(string requestedAction)
        {
            return requestedAction.Split('/').Last();
        }

        public override bool AllowsExecutionOf(object command)
        {
            var requestedCommandName = GetRequestedCommandName(command);

            return (from subjectAlias in subjectCleaner.AliasesFor(AllowedSubject)
                from actionAlias in actionCleaner.AliasesFor(AllowedAction)
                where Regex.IsMatch(requestedCommandName, actionAlias, RegexOptions.IgnoreCase)
                      && Regex.IsMatch(requestedCommandName, subjectAlias, RegexOptions.IgnoreCase)
                select subjectAlias)
                .Any();
        }

        private string GetRequestedCommandName(object command)
        {

            if (command is string)
                return actionCleaner.Clean(subjectCleaner.Clean(command.ToString()));

            var commandType = command.GetType();
            var attribute = (AuthorizeIfICanAttribute[])commandType.GetCustomAttributes(typeof(AuthorizeIfICanAttribute), true);
            return attribute.Length == 0
                ? commandType.Name
                : attribute.First().Action + attribute.First().Subject;
        }

    }
}