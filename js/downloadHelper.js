function downloadFile(filename, base64String) {
    const link = document.createElement('a');
    link.download = filename;
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + base64String;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
