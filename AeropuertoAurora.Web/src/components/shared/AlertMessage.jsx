export default function AlertMessage({
  message,
  type = 'success'
}) {
  if (!message) return null;

  return (
    <div className={`connection-alert ${type}-alert`}>
      {message}
    </div>
  );
}