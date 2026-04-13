export const formatSeconds = (totalSeconds: number) => {
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = Math.floor(totalSeconds % 60);
  let result = "";
  if (hours > 0) {
    result += `${hours}h `;
  }
  if (minutes > 0) {
    result += `${minutes}m `;
  }
  if (seconds > 0) {
    result += `${seconds}s `;
  } else if (hours === 0 && minutes === 0) {
    result += "0s ";
  }
  return result.trim();
};
